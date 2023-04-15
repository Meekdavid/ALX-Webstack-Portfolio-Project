using catalogueService.Database.DBsets;
using catalogueService.requestETresponse;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace catalogueService.Interfaces
{
    public interface ISqlprocess
    {
        public Task<databaseResult> insert_Update(string CommandQuery, CommandType cmdType, SqlParameter[] param);
        public Task<databaseResult> retrieveRecords(string CommandQuery, CommandType cmdType, SqlParameter[] param);
    }
}
