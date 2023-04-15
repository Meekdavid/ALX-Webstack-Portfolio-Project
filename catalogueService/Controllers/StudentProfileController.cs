using catalogueService.Database.DBsets;
using catalogueService.Models;
using catalogueService.requestETresponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using AutoMapper;
using catalogueService.Database.DBContextFiles;
using catalogueService.Interfaces;
using catalogueService.Database;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using static Org.Jsoup.Select.Evaluator;

namespace catalogueService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentProfileController : ControllerBase
    {
        private readonly IUser _userRep;
        private readonly IFee _program;
        private readonly IAdmin _admin;
        private readonly ICategory _category;
        private readonly ICustomer _customer;
        private readonly IJsonFormatter _JsonFormatter;
        private readonly ISqlprocess _Sqlprocess;
        private readonly IMapper _mapper;
        private readonly catalogueDBContext _dbcontext;
        private readonly ISqlprocess _databaseHandler;
        private readonly ILogger<StudentProfileController> _logger;

        public StudentProfileController(IUser userRep, IAdmin admin, ICategory category, ICustomer customer, IJsonFormatter jsonFormatter, IFee program,
            ISqlprocess sqlprocess, IMapper mapper, catalogueDBContext dbcontext, ISqlprocess databaseHandler, ILogger<StudentProfileController> logger)
        {
            _userRep = userRep;
            _admin = admin;
            _category = category;
            _customer = customer;
            _JsonFormatter = jsonFormatter;
            _Sqlprocess = sqlprocess;
            _mapper = mapper;
            _dbcontext = dbcontext;
            _databaseHandler = databaseHandler;
            _logger = logger;
            _program= program;
        }


        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = "Super Admin, Student")]
        public async Task<IActionResult> AddStudentAsync([FromBody] studentRequest Mboko)
        {
            try
            {
                var thisuser = await _userRep.GetByIdAsync(Mboko.userId);
                //var thisuser = GetCurrentUser();
                if (thisuser.userName != Mboko.emailAddress)
                {
                    return Ok("User Name Mismatch, Ensure you are using a valid User ID");
                }

                var domainUser = _mapper.Map<student>(Mboko);

                // Pass details to Repositpory
                domainUser = await _admin.AddStudentAsync(domainUser);
                if (domainUser == null)
                {
                    return NotFound();
                }

                //Assign Student role to this User
                var userDetails = await _userRep.GetByIdAsync(thisuser.userId);
                
                userModel thisUser = new userModel();

                thisUser.role = "Student";
                thisUser.wallet = userDetails.wallet;
                thisUser.password = userDetails.password;
                thisUser.phoneNumber = userDetails.phoneNumber;
                thisUser.lastName = userDetails.lastName;
                thisUser.firstName = userDetails.firstName;
                thisUser.userName = userDetails.userName;
                thisUser.locationId = userDetails.locationId;
                thisUser.typeId = userDetails.typeId;
                thisUser.userId = userDetails.userId;

                var userDomainN = _mapper.Map<users>(thisUser);

                var updatedUser = await _userRep.UpdateAsync(userDetails.userId, userDomainN);
                //Role has been successfully assigned, No let convey the info to the user

                // Convert back to DTO
                var customerDTO = _mapper.Map<studentModel>(domainUser);

                return Ok(new Response { response = $"Student '{customerDTO.firstName} {customerDTO.lastName}' successfully registered" });
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin, Student")]
        [Route("View All Students")]
        public async Task<IActionResult> GetStudentAsync()
        {
            var (IsSucess, studentDomain) = await _admin.GetStudentAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            var studentDTO = _mapper.Map<IEnumerable<studentModel>>(studentDomain);
            return Ok(studentDTO);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin, Student, Teacher")]
        [Route("View Students by Programs")]
        public async Task<IActionResult> GetStudentByProgramAsync(int programID)
        {
            var program = (await _program.GetByIdAsync(programID)).name;
            var query = "Select * from  Students where program = @program";

            SqlParameter[] Params = new SqlParameter[]
            {
                   new SqlParameter("@program",program)
            };

            var studentsTable = await _databaseHandler.retrieveRecords(query, CommandType.Text, Params);

            if (!studentsTable.queryIsSuccessful)
            {
                _logger.LogInformation($"An error occurres while retrieving student details with query: {query}");
                return Ok("Unable to retrieve student details for customer");
            }
            _logger.LogInformation($"Students table successfully retrieved for query: {query}");

            var returnObject = await _JsonFormatter.JsonFormat(studentsTable.objectValue);

            if (returnObject == "" || returnObject == null)
            {
                return Ok("There are no students who are currently enrolled for this program");
            }

            return Ok(returnObject);

        }

        [HttpGet]
        [Route("View Profile")]
        [Authorize(Roles = "Super Admin, Student")]
        public async Task<IActionResult> GetStudentByIdAsync(int regNo)
        {
            var repoUser = await _admin.GetStudentByIdAsync(regNo);
            if (repoUser == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var userDTO = _mapper.Map<studentModel>(repoUser);

            return Ok(userDTO);
        }

        [HttpPost]
        [Route("Withdraw Admission")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> WithdrawStudentAsync(long regNo)
        {
            try
            {
                var domainUser = await _admin.WithdrawStudentAsync(regNo);
                if (domainUser == null)
                {
                    return NotFound();
                }

                return Ok(new Response { response = $"Student Successfully withdrawn" });
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
        }

        [HttpGet]
        [Route("View Available Programs")]
        [Authorize(Roles = "Super Admin, Student")]
        public async Task<IActionResult> GetProgramsAsync()
        {
            return Ok(await _dbcontext.Fees.ToListAsync());
        }

        [HttpGet]
        [Route("View Available Courses")]
        [Authorize(Roles = "Super Admin, Student")]
        public async Task<IActionResult> GetCoursesAsync()
        {
            return Ok(await _dbcontext.Courses.ToListAsync());
        }

        [HttpGet]
        [Route("View Available Exams")]
        [Authorize(Roles = "Super Admin, Student")]
        public async Task<IActionResult> GetExamsAsync()
        {
            return Ok(await _dbcontext.Exams.ToListAsync());
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
