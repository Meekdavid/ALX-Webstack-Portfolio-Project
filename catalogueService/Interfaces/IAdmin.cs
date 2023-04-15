using catalogueService.Database;
using catalogueService.Database.DBsets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace catalogueService.Interfaces
{
    public interface IAdmin
    {
        public Task<(bool IsSuccess, IEnumerable<admin> admins)> GetAdminAsync();
        public Task<(bool IsSuccess, IEnumerable<student> students)> GetStudentAsync();
        public Task<(bool IsSuccess, IEnumerable<Teacher> teachers)> GetTeacherAsync();
        public Task<(bool IsSuccess, IEnumerable<Exam> exams)> FetchExamRecordsAsync();
        public Task<admin> GetAdminByIdAsync(int id);
        public Task<student> GetStudentByIdAsync(int id);
        public Task<Teacher> GetTeacherByIdAsync(int id);
        public Task<Exam> GetExamByIdAsync(int id);
        public Task<admin> AddAdminAsync(admin user);
        public Task<Exam> CreateExamAsync(Exam attributesi);
        public Task<student> AddStudentAsync(student user);
        public Task<Teacher> AddTeacherAsync(Teacher user);
        public Task<string> GradeStudentAsync(string id);
        public Task<student> WithdrawStudentAsync(long id);
    }
}
