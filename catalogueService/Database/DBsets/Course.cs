using System;
using System.ComponentModel.DataAnnotations;

namespace catalogueService.Database.DBsets
{
    public class Course
    {
        [Key]
        public int courseId { get; set; }
        [StringLength(50)]
        public string courseTitle { get; set; }
        [StringLength(50)]
        public DateTime? createdOn { get; set; }
        public int programId { get; set; }
        public Fee? program { get; set; }
    }
}
