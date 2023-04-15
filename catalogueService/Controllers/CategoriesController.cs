using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using catalogueService.Interfaces;
using catalogueService.Repositories;
using AutoMapper;
using catalogueService.Database;
using catalogueService.Models;
using catalogueService.requestETresponse;
using System;
using Microsoft.AspNetCore.Authorization;
using catalogueService.Database.DBContextFiles;
using catalogueService.Database;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace catalogueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Super Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategory _categoryRep;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategory _categoryRep, IMapper _mapper, ILogger<CategoriesController> logger)
        {
            this._categoryRep= _categoryRep;
            this._mapper= _mapper;
            this._logger = logger;
        }

        [HttpGet("All Categories")]
        //[Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            string classMethod = "GetAllUsers";
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                _logger.LogInformation($"The current user details retrieved is:\r\nUserName: {currentUser.userName}\r\nFirstName: {currentUser.firstName}" +
                    $"\r\nLastName: {currentUser.lastName}");
                if (currentUser.role.ToString().ToUpper() == "SUPER ADMIN")
                {
                    _logger.LogInformation($"This User {currentUser.userName} is a SUPER ADMIN");
                    var (IsSucess, mbokoDomain) = await _categoryRep.GetCategoryAsync();
                    if (!IsSucess)
                    {
                        return NotFound();
                    }
                    var mbokoDTO = _mapper.Map<IEnumerable<categoryModel>>(mbokoDomain);
                    return Ok(mbokoDTO);
                }
                else
                {
                    _logger.LogInformation($"This User with userID {currentUser.userId} is not allowed to view this resource");
                    return Ok("This can only be accessed by an Administrator");
                }
            }
            return Ok("Kindly register/login for access");
        }

        [HttpPost]
        [Route("Add a Category")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] categoryRequest Mboko)
        {
            try
            {
                var currentUser = GetCurrentUser();
                if (currentUser != null)
                {
                    _logger.LogInformation($"Current User found with a user ID of {currentUser.userId}");
                    if (currentUser.role.ToString().ToUpper() == "SUPER ADMIN")
                    {
                        _logger.LogInformation($"User ID: {currentUser.userId} is an Administrator");
                        var domainCategory = _mapper.Map<category>(Mboko);

                        // Pass details to Repositpory
                        domainCategory = await _categoryRep.AddCategoryAsync(domainCategory);
                        if (domainCategory == null)
                        {
                            return NotFound();
                        }
                        // Convert back to DTO
                        var categoryDTO = _mapper.Map<categoryModel>(domainCategory);
                        _logger.LogInformation($"Category successfully added by a user with a user ID of {currentUser.userId}");
                        return Ok(new Response { response = $"Category added successfully with category ID of {categoryDTO.categoryId}" });
                    }
                    else
                    {
                        _logger.LogInformation($"This resource could not be accessed by the user with the user ID of {currentUser.userId}");
                        return Ok("This can only be accessed by an Administrator");
                    }
                }
                _logger.LogInformation($"This user with ID of {currentUser.userId} wasn't logged in");
                return Ok("Kindly register/login for access");
                
            }
            catch (Exception oxg)
            {
                _logger.LogError(oxg.StackTrace, $"An error occured {oxg}");
                return BadRequest(oxg.Message);
            }
        }

        [HttpGet]
        [Route("Get Category by ID")]
        [ActionName("GetWalkDifficultyById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                _logger.LogInformation($"Current User found with a user ID of {currentUser.userId}");
                if (currentUser.role.ToString().ToUpper() == "SUPER ADMIN")
                {
                    _logger.LogInformation($"User ID: {currentUser.userId} is an Administrator");
                    var repoCategory = await _categoryRep.GetByIdAsync(id);
                    if (repoCategory == null)
                    {
                        return NotFound();
                    }

                    //Convert Domain to DTO
                    var categoryDTO = _mapper.Map<categoryModel>(repoCategory);
                    _logger.LogInformation($"Category with category ID {categoryDTO.categoryId} successfully fetched for user ID: {currentUser.userId}");
                    return Ok(categoryDTO);
                }
                else
                {
                    _logger.LogInformation($"This resource could not be accessed by the user with the user ID of {currentUser.userId}");
                    return Ok("This can only be accessed by an Administrator");
                }
            }
            _logger.LogInformation($"This user with ID of {currentUser.userId} wasn't logged in");
            return Ok("Kindly register/login for access");
           
        }

        private users GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                return new users
                {
                    userName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    //userName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    firstName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    lastName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    role = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                };

            }
            return null;
        }
    }
}
