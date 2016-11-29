using EntropyExtension.Core;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace EntropyExtension.SqlServer
{
    public class SqlServerEntropyStore : IEntropyStore, IDisposable
    {

        private static SqlServerEntropyOptions _options;
        private SqlConnection _connection;

        public SqlServerEntropyStore()
        {
        }

        ~SqlServerEntropyStore()
        {
            Dispose();
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
        private SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(Options.ConnectionString);
                    _connection.Open();
                }

                return _connection;
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            GC.SuppressFinalize(this);
        }

        public void EnsureCreated()
        { }

        public void Log(EntropyLogInfo info)
        {
            var query = "INSERT INTO [dbo].[EntropyLog] " +
                       "(LogId,  LogLevel,  ApplicationName,  Name,  TimeUtc,  LocalTime,  Exception, StateInfo) " +
                   "VALUES " +
                       "(@LogId, @LogLevel, @ApplicationName, @Name, @TimeUtc, @LocalTime, @Exception, @StateInfo); ";

            if (info.HttpInfo != null)
            {
                query += "INSERT INTO [dbo].[EntropyLog_HttpInfo] " +
                       "(HttpInfoId,  LogId,  ContentType,  Cookies,  Headers,  Host,  Method,  [Path], Protocol,  Query,  RequestID,  Scheme,  StatusCode,  [User])" +
                   "VALUES" +
                       "(@HttpInfoId, @LogId, @ContentType, @Cookies, @Headers, @Host, @Method, @Path,  @Protocol, @Query, @RequestID, @Scheme, @StatusCode, @User);";
            }


            using (var cmd = new SqlCommand(query, Connection))
            {
                var logId = Guid.NewGuid();

                var ex = JsonConvert.SerializeObject(info.Exception);

                cmd.Parameters.Add(new SqlParameter("@LogId", logId));
                cmd.Parameters.Add(new SqlParameter("@LogLevel", info.LogLevel.ToString()));
                cmd.Parameters.Add(new SqlParameter("@ApplicationName", info.ApplicationName));
                cmd.Parameters.Add(new SqlParameter("@Name", info.Name));
                cmd.Parameters.Add(new SqlParameter("@TimeUtc", info.Time));
                cmd.Parameters.Add(new SqlParameter("@LocalTime", info.LocalTime));
                cmd.Parameters.Add(new SqlParameter("@Exception", info.Exception == null ? (object)DBNull.Value : ex));
                cmd.Parameters.Add(new SqlParameter("@StateInfo", string.IsNullOrWhiteSpace(info.State) ? (object)DBNull.Value : info.State));

                if (info.HttpInfo != null)
                {
                    var cookies = JsonConvert.SerializeObject(info.HttpInfo.Cookies.ToDictionary(kpv => kpv.Key, kpv => kpv.Value));
                    var headers = JsonConvert.SerializeObject(info.HttpInfo.Headers.ToDictionary(kpv => kpv.Key, kpv => kpv.Value));

                    cmd.Parameters.Add(new SqlParameter("@HttpInfoId", Guid.NewGuid()));
                    cmd.Parameters.Add(new SqlParameter("@ContentType", (object)info.HttpInfo.ContentType ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Cookies", cookies));
                    cmd.Parameters.Add(new SqlParameter("@Headers", headers));
                    cmd.Parameters.Add(new SqlParameter("@Host", info.HttpInfo.Host.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@Method", (object)info.HttpInfo.Method ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Path", info.HttpInfo.Path.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@Protocol", (object)info.HttpInfo.Protocol ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Query", info.HttpInfo.Query.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@RequestID", (object)info.HttpInfo.RequestID ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Scheme", (object)info.HttpInfo.Scheme ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@StatusCode", info.HttpInfo.StatusCode));
                    cmd.Parameters.Add(new SqlParameter("@User", (object)info.HttpInfo.User.Identity.Name ?? DBNull.Value));
                }

                cmd.ExecuteNonQuery();
            }

        }
    }
}
