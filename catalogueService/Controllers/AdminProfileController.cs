using AutoMapper;
using catalogueService.Database;
using catalogueService.Database.DBContextFiles;
using catalogueService.Database.DBsets;
using catalogueService.Interfaces;
using catalogueService.Models;
using catalogueService.requestETresponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using static Org.Jsoup.Select.Evaluator;

namespace catalogueService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminProfileController : ControllerBase
    {
        //Initialize dependency
        private readonly IUser _userRep;
        private readonly IAdmin _admin;
        private readonly ICategory _category;
        private readonly ICustomer _customer;
        private readonly IJsonFormatter _JsonFormatter;
        private readonly ISqlprocess _Sqlprocess;
        private readonly IMapper _mapper;
        private readonly catalogueDBContext _dbcontext;

        public AdminProfileController(IUser userRep, IAdmin admin, ICategory category, ICustomer customer, IJsonFormatter jsonFormatter, ISqlprocess sqlprocess, IMapper mapper, catalogueDBContext dbcontext)
        {
            //Inject dependecies in class's constructor
            _userRep = userRep;
            _admin = admin;
            _category = category;
            _customer = customer;
            _JsonFormatter = jsonFormatter;
            _Sqlprocess = sqlprocess;
            _mapper = mapper;
            _dbcontext= dbcontext;
        }

        [HttpGet]
        [Route("Get All Admins")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetAdminAsync()
        {
            //Get Admin details from admin repo
            var (IsSucess, adminDomain) = await _admin.GetAdminAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            //Convert the admin details to model passed back to client
            var adminDTO = _mapper.Map<IEnumerable<adminModel>>(adminDomain);
            return Ok(adminDTO);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin, Teacher")]
        [Route("Get All Students")]
        public async Task<IActionResult> GetStudentAsync()
        {
            //Get Student details from student repo
            var (IsSucess, studentDomain) = await _admin.GetStudentAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            //Convert the student details to model passed back to client
            var studentDTO = _mapper.Map<IEnumerable<studentModel>>(studentDomain);
            return Ok(studentDTO);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        [Route("Get All Teachers")]
        public async Task<IActionResult> GetTeacherAsync()
        {
            //Get Teacher details from admin repo
            var (IsSucess, teacherDomain) = await _admin.GetTeacherAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            //Convert the teacher details to model passed back to client
            var teacherDTO = _mapper.Map<IEnumerable<teacherModel>>(teacherDomain);
            return Ok(teacherDTO);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin, Teacher")]
        [Route("Fetch Exam Records")]
        public async Task<IActionResult> FetchExamRecordsAsync()
        {
            //Get Exam details from admin repo
            var (IsSucess, examDomain) = await _admin.FetchExamRecordsAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            //Convert the exams details to model passed back to client
            var examDTO = _mapper.Map<IEnumerable<ExamModel>>(examDomain);
            return Ok(examDTO);
        }

        [HttpGet]
        [Route("Get single Admin")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetAdminByIdAsync(int id)
        {
            //Get Admin details from admin repo
            var repoUser = await _admin.GetAdminByIdAsync(id);
            if (repoUser == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var userDTO = _mapper.Map<adminModel>(repoUser);

            return Ok(userDTO);
        }

        [HttpGet]
        [Route("Get single student")]
        [Authorize(Roles = "Super Admin, Teacher, Student")]
        public async Task<IActionResult> GetStudentByIdAsync(int id)
        {
            //Get student details from admin repo
            var repoUser = await _admin.GetStudentByIdAsync(id);
            if (repoUser == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var userDTO = _mapper.Map<studentModel>(repoUser);

            return Ok(userDTO);
        }

        [HttpGet]
        [Route("Get single Teacher")]
        [Authorize(Roles = "Super Admin, Teacher")]
        public async Task<IActionResult> GetTeacherByIdAsync(int id)
        {
            //Get Teacher details from admin repo
            var repoUser = await _admin.GetTeacherByIdAsync(id);
            if (repoUser == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var userDTO = _mapper.Map<teacherModel>(repoUser);

            return Ok(userDTO);
        }

        [HttpGet]
        [Route("Get single Exam")]
        [Authorize(Roles = "Super Admin, Teacher, Student")]
        public async Task<IActionResult> GetExamByIdAsync(int id)
        {
            //Get Exam details from admin repo
            var repoUser = await _admin.GetExamByIdAsync(id);
            if (repoUser == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var userDTO = _mapper.Map<ExamModel>(repoUser);

            return Ok(userDTO);
        }

        [HttpPost]
        [Route("Add new Admin")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> AddAdminAsync([FromBody] adminRequest Mboko)
        {
            try
            {
                var domainUser = _mapper.Map<admin>(Mboko);

                // Pass details to Repositpory
                domainUser = await _admin.AddAdminAsync(domainUser);
                if (domainUser == null)
                {
                    return NotFound();
                }
                // Convert back to DTO
                var customerDTO = _mapper.Map<adminModel>(domainUser);

                return Ok(new Response { response = $"New Admin, {customerDTO.firstName} {customerDTO.lastName} added successfully" });
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
        }

        [HttpPost]
        [Route("Add an Exam")]
        [Authorize(Roles = "Super Admin, Teacher, Student")]
        public async Task<IActionResult> CreateExamAsync([FromBody] examRequest Mboko)
        {
            try
            {
                var domainUser = _mapper.Map<Exam>(Mboko);

                // Pass details to Repositpory
                domainUser = await _admin.CreateExamAsync(domainUser);
                if (domainUser == null)
                {
                    return NotFound();
                }
                // Convert back to DTO
                var customerDTO = _mapper.Map<ExamModel>(domainUser);

                return Ok(new Response { response = $"New Exam, '{customerDTO.examName}' added successfully" });
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
        }

        [HttpPost]
        [Route("Add Student")]
        [Authorize(Roles = "Super Admin, Teacher")]
        public async Task<IActionResult> AddStudentAsync([FromBody] studentRequest Mboko)
        {
            var returner = new object();
            try
            {
                //First Check if this user exists;
                var users = await _dbcontext.Users.ToListAsync();
                foreach (var user in users)
                {
                    if (user.userId == Mboko.userId)
                    {
                        var domainUser = _mapper.Map<student>(Mboko);

                        var thisStudent = await _dbcontext.Students.ToListAsync();
                        foreach(var student in thisStudent)
                        {
                            if (student.emailAddress == Mboko.emailAddress && student.firstName == Mboko.firstName && student.lastName == Mboko.lastName)
                            {
                                return Ok("User already registered as a student");
                            }
                        }

                        // Pass details to Repositpory
                        domainUser = await _admin.AddStudentAsync(domainUser);
                        if (domainUser == null)
                        {
                            returner = NotFound();
                        }
                        // Convert back to DTO
                        var customerDTO = _mapper.Map<studentModel>(domainUser);

                        returner = Ok(new Response { response = $"Student '{customerDTO.firstName} {customerDTO.lastName}' added successfully" });

                    }
                    returner = Ok($"There are no users with ID: {Mboko.userId}, Or information provided doesn't match the UserID");
                }

                
            }
            catch (Exception oxg)
            {
                returner = BadRequest(oxg.Message);
            }

            return (IActionResult)returner;
        }

        [HttpPost]
        [Route("Add Teacher")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> AddTeacherAsync([FromBody] teacherRequest Mboko)
        {
            var returner = new object();
            try
            {
                var users = await _dbcontext.Users.ToListAsync();
                foreach (var user in users)
                {
                    if (user.userId == Mboko.userId)
                    {
                        var domainUser = _mapper.Map<Teacher>(Mboko);

                        var thisTeacher = await _dbcontext.Students.ToListAsync();
                        foreach (var teacher in thisTeacher)
                        {
                            if (teacher.emailAddress == Mboko.emailAddress && teacher.firstName == Mboko.firstName && teacher.lastName == Mboko.lastName)
                            {
                                return Ok("User already registered as a Teacher");
                            }
                        }

                        // Pass details to Repositpory
                        domainUser = await _admin.AddTeacherAsync(domainUser);
                        if (domainUser == null)
                        {
                            returner = NotFound();
                        }
                        // Convert back to DTO
                        var customerDTO = _mapper.Map<teacherModel>(domainUser);

                        returner = Ok(new Response { response = $"Teacher {customerDTO.firstName} {customerDTO.lastName} added successfully" });
                        
                    }
                    returner = Ok($"There are no users with ID: {Mboko.userId}");
                }
                
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
            return (IActionResult)returner;
        }

        [HttpPost]
        [Route("Grade Students")]
        [Authorize(Roles = "Super Admin, Teacher")]
        public async Task<IActionResult> GradeStudentAsync(string idPIPEgpa)
        {
            try
            {
                //Get student details from the repository
                var domainUser = await _admin.GradeStudentAsync(idPIPEgpa);
                if (domainUser == "FALSE")
                {
                    return Ok(new Response { response = $"Student Not Successfully graded" });
                }
                return Ok(new Response { response = $"Student Successfully graded" });
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
        }

        [HttpPost]
        [Route("Withdraw a Student")]
        [Authorize(Roles = "Super Admin, Teacher")]
        public async Task<IActionResult> WithdrawStudentAsync(long regNo)
        {
            try
            {
                //Ammend database, to implement the deletion of the student
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
        [Route("View withdrawn students")]
        [Authorize(Roles = "Super Admin, Teacher")]
        public async Task<IActionResult> WithdrawStudentAsync()
        {
            //Get the list of withdrawn students
            var thisStudent = await _dbcontext.withdrawnStudents.ToListAsync();

            return Ok(thisStudent);
        }

        [HttpPost]
        [Route("Assign Role")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> roleAssignmentAsync(roleRequest request)
        {
            try
            {
                //Get the required user by their Id
                var domainUser = await _userRep.GetByIdAsync(request.userID);
                if (domainUser == null)
                {
                    return NotFound();
                }

                userModel thisUser = new userModel();
                switch (request.roleID)
                {
                    case 1:
                        thisUser.role = "SUPER ADMIN";
                        break;

                    case 2:
                        thisUser.role = "Teacher";
                        break; 
                    
                    case 3:
                        thisUser.role = "Student";
                        break;
                }

                thisUser.wallet = domainUser.wallet;
                thisUser.password = domainUser.password;
                thisUser.phoneNumber = domainUser.phoneNumber;
                thisUser.lastName = domainUser.lastName;
                thisUser.firstName = domainUser.firstName;
                thisUser.userName = domainUser.userName;
                thisUser.locationId = domainUser.locationId;
                thisUser.typeId = domainUser.typeId;
                thisUser.userId = domainUser.userId;

                var userDomainN = _mapper.Map<users>(thisUser);

                //Update the database witht the ammended user model
                var updatedUser = await _userRep.UpdateAsync(request.userID, userDomainN);

                return Ok(new Response { response = $"user role sucessfully assigned for '{domainUser.userName}'" });
            }
            catch (Exception oxg)
            {
                return BadRequest(oxg.Message);
            }
        }
    }
}
