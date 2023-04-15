using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.Database.DBsets
{
    public class admin
    {
        [Key] public int adminID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [EmailAddress] public string emailAddress { get; set; }
        [Phone] public string phoneNo { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string gender { get; set; }
        public string Address { get; set; }
        public string department { get; set; }
        public string? departmentalRole { get; set; }
        public string? accessLevel { get; set; }
        public int? yearsOfExperience { get; set; }
        public string hireDate { get; set; } = DateTime.Now.ToString();
        public string? employmentStatus { get; set; }
        public string? certifications { get; set; }
        public string isAdmin { get; set; } = "FALSE";
        public int userId { get; set; }
        public users? _users { get; set; }
    }
}
