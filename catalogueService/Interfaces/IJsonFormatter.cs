using catalogueService.requestETresponse;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace catalogueService.Interfaces
{
    public interface IJsonFormatter
    {
        public Task<string> JsonFormat(DataTable objectValue);
    }
}
