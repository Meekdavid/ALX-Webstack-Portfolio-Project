using System.ComponentModel.DataAnnotations;

namespace catalogueService.requestETresponse
{
    public class examRequest
    {
        public int examId { get; set; }
        public string examName { get; set; }
        public string examDescription { get; set; }
        public string invigilator { get; set; }
        public int passScore { get; set; }
    }
}
