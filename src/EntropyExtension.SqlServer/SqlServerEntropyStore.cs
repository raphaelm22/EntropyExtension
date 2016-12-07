using EntropyExtension.Core;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntropyExtension.SqlServer
{
    public class SqlServerEntropyStore : IEntropyStore
    {

        private static SqlServerEntropyOptions _options;

        public SqlServerEntropyStore()
        {
        }

        internal static SqlServerEntropyOptions Options
        {
            get
            {
                if (_options == null)
                    _options = new SqlServerEntropyOptions();

                return _options;
            }
            set
            {
                _options = value;
            }
        }

        public void EnsureCreated()
        { }

        public void Log(EntropyLogInfo info)
        {
            using (var conn = new SqlConnection(Options.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(CreateQuery(info), conn))
                {
                    FillParameters(info, cmd);
                    ExecuteQuery(cmd);
                }
            }
        }

        private static string CreateQuery(EntropyLogInfo info)
        {
            var query = new StringBuilder();
            query.AppendLine("SET NOCOUNT ON;");

            query.AppendLine("INSERT INTO [dbo].[EntropyLog] " +
                       "(LogId,  LogLevel,  ApplicationName,  Name,  LocalTime,  Exception,  StateInfo,  MachineName) " +
                   "VALUES " +
                       "(@LogId, @LogLevel, @ApplicationName, @Name, @LocalTime, @Exception, @StateInfo, @MachineName);");

            if (info.HttpInfo != null)
            {
                query.AppendLine("INSERT INTO [dbo].[EntropyLog_HttpInfo] " +
                       "(HttpInfoId,  LogId,  ContentType,  Cookies,  Headers,  Host,  Method,  [Path], Protocol,  Query,  RequestID,  Scheme,  StatusCode,  [User])" +
                   "VALUES" +
                       "(@HttpInfoId, @LogId, @ContentType, @Cookies, @Headers, @Host, @Method, @Path,  @Protocol, @Query, @RequestID, @Scheme, @StatusCode, @User);");
            }

            return query.ToString();
        }

        private static void FillParameters(EntropyLogInfo info, SqlCommand cmd)
        {
            var logId = Guid.NewGuid();

            var ex = JsonConvert.SerializeObject(info.Exception);

            cmd.Parameters.Add(new SqlParameter("@LogId", logId));
            cmd.Parameters.Add(new SqlParameter("@LogLevel", info.LogLevel.ToString()));
            cmd.Parameters.Add(new SqlParameter("@ApplicationName", info.ApplicationName));
            cmd.Parameters.Add(new SqlParameter("@Name", info.Name));
            cmd.Parameters.Add(new SqlParameter("@LocalTime", info.Time));
            cmd.Parameters.Add(new SqlParameter("@Exception", info.Exception == null ? (object)DBNull.Value : ex));
            cmd.Parameters.Add(new SqlParameter("@StateInfo", string.IsNullOrWhiteSpace(info.State) ? (object)DBNull.Value : info.State));
            cmd.Parameters.Add(new SqlParameter("@MachineName", info.MachineName));

            if (info.HttpInfo != null)
            {
                var cookies = info.HttpInfo.Cookies.Any()
                    ? JsonConvert.SerializeObject(info.HttpInfo.Cookies.ToDictionary(kpv => kpv.Key, kpv => kpv.Value))
                    : null;
                var headers = info.HttpInfo.Headers.Any()
                    ? JsonConvert.SerializeObject(info.HttpInfo.Headers.ToDictionary(kpv => kpv.Key, kpv => kpv.Value))
                    : null;
                var urlQuery = info.HttpInfo.Query.HasValue
                    ? info.HttpInfo.Query.ToString()
                    : null;

                cmd.Parameters.Add(new SqlParameter("@HttpInfoId", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@ContentType", (object)info.HttpInfo.ContentType ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Cookies", (object)cookies ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Headers", (object)headers ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Host", info.HttpInfo.Host.ToString()));
                cmd.Parameters.Add(new SqlParameter("@Method", (object)info.HttpInfo.Method ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Path", info.HttpInfo.Path.ToString()));
                cmd.Parameters.Add(new SqlParameter("@Protocol", (object)info.HttpInfo.Protocol ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Query", (object)urlQuery ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@RequestID", (object)info.HttpInfo.RequestID ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Scheme", (object)info.HttpInfo.Scheme ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@StatusCode", info.HttpInfo.StatusCode));
                cmd.Parameters.Add(new SqlParameter("@User", (object)info.HttpInfo.User.Identity.Name ?? DBNull.Value));
            }
        }

        private static void ExecuteQuery(SqlCommand cmd)
        {
            for (int i = 0; i <= 5; i++)
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    break;
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number != -2) //Not timeout occurred
                        throw;

                    Task.Delay(3 * 1000);
                }
            }
        }
    }
}
