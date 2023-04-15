using AutoMapper;
using catalogueService.Database;
using catalogueService.Database.DBContextFiles;
using catalogueService.Database.DBsets;
using catalogueService.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace catalogueService.Repositories
{
    public class adminRepo : IAdmin
    {
        private readonly catalogueDBContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly ISqlprocess _databaseHandler;

        public adminRepo(catalogueDBContext dbcontext, IMapper mapper, ISqlprocess databaseHandler)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
            _databaseHandler = databaseHandler;
        }
        public async Task<admin> AddAdminAsync(admin user)
        {
            await _dbcontext.Admins.AddAsync(user);
            await _dbcontext.SaveChangesAsync();
            return user;
        }

        public async Task<student> AddStudentAsync(student user)
        {
            await _dbcontext.Students.AddAsync(user);
            await _dbcontext.SaveChangesAsync();
            return user;
        }

        public async Task<Teacher> AddTeacherAsync(Teacher user)
        {
            await _dbcontext.Teachers.AddAsync(user);
            await _dbcontext.SaveChangesAsync();
            return user;
        }

        public async Task<Exam> CreateExamAsync(Exam attributesi)
        {
            await _dbcontext.Exams.AddAsync(attributesi);
            await _dbcontext.SaveChangesAsync();
            return attributesi;
        }

        public async Task<(bool IsSuccess, IEnumerable<Exam> exams)> FetchExamRecordsAsync()
        {
            try
            {
                var results = await _dbcontext.Exams.ToListAsync();
                return (true, results);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<admin> admins)> GetAdminAsync()
        {
            try
            {
                var results = await _dbcontext.Admins.ToListAsync();
                return (true, results);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<admin> GetAdminByIdAsync(int id)
        {
            return await _dbcontext.Admins.FirstOrDefaultAsync(x => x.adminID == id);
        }

        public async Task<Exam> GetExamByIdAsync(int id)
        {
            return await _dbcontext.Exams.FirstOrDefaultAsync(x => x.examId == id);
        }

        public async Task<(bool IsSuccess, IEnumerable<student> students)> GetStudentAsync()
        {
            try
            {
                var results = await _dbcontext.Students.ToListAsync();
                return (true, results);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<student> GetStudentByIdAsync(int regNo)
        {
            return await _dbcontext.Students.FirstOrDefaultAsync(x => x.regNo == regNo);
        }

        public async Task<(bool IsSuccess, IEnumerable<Teacher> teachers)> GetTeacherAsync()
        {
            try
            {
                var results = await _dbcontext.Teachers.ToListAsync();
                return (true, results);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            return await _dbcontext.Teachers.FirstOrDefaultAsync(x => x.teacherID == id);
        }

        public async Task<string> GradeStudentAsync(string userIdGPA)
        {
            string[] splitted = userIdGPA.Split('|');
            int userId = int.Parse(splitted[0]);
            int GPA = int.Parse(splitted[1]);
            try
            {
                var query2 = "Update Students set GPA = @GPA where userId = @userId";
                SqlParameter[] Params1 = new SqlParameter[]
                {
                    new SqlParameter("@userId",userId),
                    new SqlParameter("@GPA",GPA)
                };

                var walletUpdate = await _databaseHandler.insert_Update(query2, CommandType.Text, Params1);
                if (!walletUpdate.queryIsSuccessful)
                {
                    return "FALSE";
                }
                return "TRUE";
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<student> WithdrawStudentAsync(long id)
        {
            withdrawnStudents newStu = new withdrawnStudents();
            var thisStudent = await _dbcontext.Students.FirstOrDefaultAsync(x => x.regNo == id);

            newStu.phoneNo = thisStudent.phoneNo;
            newStu.dateOfBirth = thisStudent.dateOfBirth;
            newStu.GPA = thisStudent.GPA;
            newStu.program = thisStudent.program;
            //newStu.regNo = thisStudent.regNo;
            newStu.gender = thisStudent.gender;
            newStu.registrationDate = thisStudent.registrationDate;
            newStu.graduationDate = thisStudent.graduationDate;
            newStu.Address = thisStudent.Address;
            newStu.emailAddress = thisStudent.emailAddress;
            newStu.coursesTaken = thisStudent.coursesTaken;
            newStu.firstName = thisStudent.firstName;
            newStu.lastName = thisStudent.lastName;
            newStu.enrollmentStatus = thisStudent.enrollmentStatus;
            newStu.userId = thisStudent.userId;


            await _dbcontext.withdrawnStudents.AddAsync(newStu);
            _dbcontext.Students.Remove(thisStudent);
            await _dbcontext.SaveChangesAsync();
            return thisStudent;
        }
    }
}
