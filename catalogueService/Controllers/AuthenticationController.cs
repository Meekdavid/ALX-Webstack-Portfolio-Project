using catalogueService.Authentication;
using catalogueService.Database.DBContextFiles;
using catalogueService.Database.DBsets;
using catalogueService.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using catalogueService.requestETresponse;
using catalogueService.Interfaces;
using catalogueService.Repositories;
using AutoMapper;
using System.Data;
using System.Data.SqlClient;
using catalogueService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JWTAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly catalogueDBContext _dbcontext;
        private readonly IUser _registerUser;
        private readonly IMapper _mapper;
        private readonly ICustomer _customerRep;

        public AuthenticateController(IConfiguration _configuration, catalogueDBContext _dbcontext, IUser _registerUser, IMapper mapper, ICustomer customerRep, ILogger<AuthenticateController> logger)
        {
            this._configuration = _configuration;
            this._dbcontext = _dbcontext;
            this._registerUser = _registerUser;
            this._mapper = mapper;
            this._customerRep = customerRep;
            this._logger = logger;
        }
        
        [Consumes("application/json")]
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel userLogin)
        {
            _logger.LogInformation($"Login request received is Username: {userLogin.userName}");
            var user = Authenticate(userLogin);
            if (user != null)
            {
                _logger.LogInformation($"Username: {userLogin.userName} successfully logged in");
                var token = Generate(user);
                return Ok(token);
            }
            _logger.LogInformation($"There is no user with Username: {userLogin.userName}");
            return NotFound("User Not Registered");

            //Create the security identity and assigned user attributes to it
            string Generate(users user)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.userName),
                new Claim(ClaimTypes.Email, user.userName),
                new Claim(ClaimTypes.GivenName, user.firstName),
                new Claim(ClaimTypes.Surname, user.lastName),
                new Claim(ClaimTypes.Role, user.role),
                new Claim(ClaimTypes.UserData, user.userId.ToString())};

                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }


            users Authenticate(LoginModel userLogin)
            {
                //Validdate the current user
                var currentUser = _dbcontext.Users.FirstOrDefault(o => o.userName.ToLower() == userLogin.userName.ToLower() && o.password == userLogin.password);
                if (currentUser != null)
                {
                    _logger.LogInformation($"The Username of the current user is: {currentUser.userName}");
                    return currentUser;
                }
                _logger.LogInformation($"There are no records for the user: {userLogin.userName}");
                return null;
            }
        }

        //[AllowAnonymous]
        [Consumes("application/json")]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] authUserRequest Mboko)
        {
            _logger.LogInformation($"About to register user details with the name {Mboko.lastName} {Mboko.firstName}");
            try
            {
                var users = await _dbcontext.Users.ToListAsync();
                foreach (var user in users)
                {
                    if (user.userName == Mboko.userName)
                    {
                        _logger.LogInformation($"The User Name '{user.userName}' already taken");
                        return Ok("Username already taken");
                    }
                }
                var domainUser = _mapper.Map<users>(Mboko);

                // Pass details to Repositpory
                domainUser = await _registerUser.AddUserAsync(domainUser);
                if (domainUser == null)
                {
                    return NotFound();
                }
                _logger.LogInformation($"User {domainUser.lastName} {domainUser.firstName} successfully registered");
                _logger.LogInformation($"About adding {Mboko.lastName} {Mboko.firstName} to the Customers table");
                var newCustomerRequest = new customer()
                {
                    firstName = domainUser.firstName,
                    lastName = domainUser.lastName,
                    phoneNumber = domainUser.phoneNumber,
                    userId = domainUser.userId,
                };
                var addCustomer = await _customerRep.AddCustomerAsync(newCustomerRequest);
                _logger.LogInformation($"User '{Mboko.lastName} {Mboko.firstName}' Successfully added to the Customer Table");
                var customerDTO = _mapper.Map<customerModel>(addCustomer);

                return Ok(new registrationResponse { response = $"Registration Sucessful", message = $"Your user ID is {customerDTO.userId}, and your customer ID is {customerDTO.customerId}" });
            }
            catch (Exception oxg)
            {
                _logger.LogError(oxg.StackTrace, $"An error occured while registering {Mboko.userName}");
                return BadRequest(oxg.Message);
            }
        }

        
    }
}