using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.requestETresponse
{
    public class studentRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        [EmailAddress] public string emailAddress { get; set; }
        [Phone] public string phoneNo { get; set; }
        public DateTime dateOfBirth { get; set; } = DateTime.Now;
        public string gender { get; set; }
        public string Address { get; set; }
        public string enrollmentStatus { get; set; }
        public string? program { get; set; }
        public string? coursesTaken { get; set; }
        public DateTime? registrationDate { get; set; } = DateTime.Now;
        public DateTime? graduationDate { get; set; } = DateTime.Now;
        public int userId { get; set; }
    }
}
