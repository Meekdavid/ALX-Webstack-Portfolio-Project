using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.requestETresponse
{
    public class adminRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string phoneNo { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string gender { get; set; }
        public string Address { get; set; }
        public string department { get; set; }
        public string? departmentalRole { get; set; }
        public int? yearsOfExperience { get; set; }
        public string hireDate { get; set; } = DateTime.Now.ToString();
        public string? employmentStatus { get; set; }
        public string? certifications { get; set; }
        public int userId { get; set; }
    }
}
