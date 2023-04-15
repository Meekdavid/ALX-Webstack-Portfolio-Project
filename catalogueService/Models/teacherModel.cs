using catalogueService.Database;
using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.Models
{
    public class teacherModel
    {
        public int teacherID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [EmailAddress] public string emailAddress { get; set; }
        [Phone] public string phoneNo { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string gender { get; set; }
        public string Address { get; set; }
        public string employmentStatus { get; set; }
        public string? subjectTaught { get; set; }
        public string? educationLevel { get; set; }
        public int? yearsOfExperience { get; set; }
        public string registrationDate { get; set; }
        public string? classSchedule { get; set; }
        public string? courseList { get; set; }
        public int userId { get; set; }
    }
}
