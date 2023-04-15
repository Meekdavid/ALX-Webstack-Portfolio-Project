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
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace catalogueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeesController : ControllerBase
    {
        private readonly IFee _FeeRep;
        private readonly IMapper _mapper;
        private readonly ILogger<FeesController> _logger;
        //private readonly catalogueDBContext _dbcontext;

        public FeesController(IMapper LampMap, IFee FeeRep, ILogger<FeesController> logger)
        {
            this._mapper = LampMap;
            this._FeeRep = FeeRep;
            this._logger= logger;
        }

        [HttpGet]
        [Route("View all Program")]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            var currentuser = GetCurrentUser();
            if (currentuser != null)
            {
                _logger.LogInformation($"The user for this session is {currentuser.lastName} {currentuser.firstName}");
                var (IsSucess, mbokoDomain) = await _FeeRep.GetFeeAsync();
                if (!IsSucess)
                {
                    return NotFound();
                }
                var mbokoDTO = _mapper.Map<IEnumerable<FeeModel>>(mbokoDomain);
                return Ok(mbokoDTO);
            }
            _logger.LogInformation($"This User {currentuser.userName} is not Authorized to view this resource");
            return Ok("User not Authorized");
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

        [HttpGet]
        [Route("View Program Fees by FeeId")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var walkDifficulty = await _FeeRep.GetByIdAsync(id);
            if (walkDifficulty == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var walkDifficultyDTO = _mapper.Map<FeeModel>(walkDifficulty);

            return Ok(walkDifficultyDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin, Teacher")]
        [Route("Add new Program Fee")]
        public async Task<IActionResult> AddFeeAsync([FromBody] FeeRequest Mboko)
        {
            try
            {
                var walk = new Fee()
                {
                    name = Mboko.name,
                    quantity = Mboko.quantity,
                    categoryId = Mboko.categoryId,
                    price = Mboko.price,
                    description = Mboko.description,

                };

                // Pass details to Repositpory
                walk = await _FeeRep.AddFeeAsync(walk);

                // Convert back to DTO
                var walkDTO = new FeeModel
                {
                    FeeId = walk.FeeId,
                    name = walk.name,
                    quantity = walk.quantity,
                    categoryId = walk.categoryId,
                    price = walk.price,
                    description = walk.description,
                };

                return Ok(new Response { response = $"Fee added successfully with Fee ID of {walkDTO.FeeId}" });
            }
            catch (Exception oxg)
            {
                _logger.LogError(oxg.StackTrace, $"An error occured: {oxg}");
                return BadRequest(oxg.Message);
            }
        }       

    }
}
