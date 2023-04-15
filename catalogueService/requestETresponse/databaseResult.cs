using System.Data;

namespace catalogueService.requestETresponse
{
    public class databaseResult
    {
        public int ResponseCode { get; set; }
        public DataTable objectValue { get; set; }
        public bool queryIsSuccessful { get; set; } = false;
    }
}
