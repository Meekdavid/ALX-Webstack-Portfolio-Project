using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.Database.DBsets
{
    public class student
    {
        [Key] public long regNo { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [EmailAddress] public string emailAddress { get; set; }
        [Phone] public string phoneNo { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string gender { get; set; }
        public string Address { get; set; }
        public string enrollmentStatus { get; set; }
        public string? program { get; set; }
        public int? GPA { get; set; }
        public string? coursesTaken { get; set; }
        public string registrationDate { get; set; }
        public string? graduationDate { get; set; }
        public int userId { get; set; }
        public users? _users { get; set; }
    }
}
