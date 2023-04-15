using catalogueService.Interfaces;
using catalogueService.requestETresponse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace catalogueService.Repositories
{
    public class SqlRepo : ISqlprocess
    {
        private readonly ILogger<SqlRepo> _logger;
        private readonly IConfiguration _configuration;

        public SqlRepo(ILogger<SqlRepo> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }
        public async Task<databaseResult> insert_Update(string CommandQuery, CommandType cmdType, SqlParameter[] param)
        {
            databaseResult result = new databaseResult();
            try
            {
                using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
                _logger.LogInformation($"SQL About executing {CommandQuery}");
                using var command = new SqlCommand(CommandQuery, connection);
                command.CommandType = cmdType;
                command.Parameters.AddRange(param);

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var rowsAffected = await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                if (rowsAffected != 0)
                {
                    result.queryIsSuccessful = true;
                    result.ResponseCode = 200;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.StackTrace, $"An error occured: {ex}");
                result.queryIsSuccessful= false;
                result.ResponseCode = 400;
            }
            return result;
        }

        public async Task<databaseResult> retrieveRecords(string CommandQuery, CommandType cmdType, SqlParameter[] param)
        {
            databaseResult result = new databaseResult();
            DataTable ds = new DataTable();
            try
            {
                using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
                _logger.LogInformation($"SQL About executing {CommandQuery}");
                using var command = new SqlCommand(CommandQuery, connection);
                command.CommandType = cmdType;
                command.Parameters.AddRange(param);

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var rowsAffected = await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                if (rowsAffected != 0)
                {
                    result.queryIsSuccessful = true;
                    result.ResponseCode = 200;
                }

                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    da.Fill(ds);
                    result.objectValue = ds;
                    _logger.LogInformation($"Method finished executing with result: {ds.ToString()}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.StackTrace, $"An error occured: {ex}");
                result.queryIsSuccessful = false;
                result.ResponseCode = 400;
            }
            return result;
        }
    }
}
