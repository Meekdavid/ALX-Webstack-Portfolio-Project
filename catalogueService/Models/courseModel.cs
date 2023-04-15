using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.Models
{
    public class courseModel
    {
        public int courseId { get; set; }
        [StringLength(50)]
        public string courseTitle { get; set; }
        [StringLength(50)]
        public DateTime? createdOn { get; set; }
        public int programId { get; set; }
    }
}
