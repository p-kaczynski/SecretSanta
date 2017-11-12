using System;
using System.Data;
using System.Data.SqlClient;
using NLog;
using SecretSanta.Common.Interface;

namespace SecretSanta.Data
{
    public abstract class BaseRepository
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public Func<string, IDbConnection> DbConnectionFactory { get; set; } = connectionString => new SqlConnection(connectionString);

        private readonly string _connectionString;

        protected BaseRepository(IConfigProvider configProvider)
        {
            _connectionString = configProvider.ConnectionString;
        }

        protected T WithConnection<T>(Func<IDbConnection, T> func)
        {
            using (var conn = DbConnectionFactory(_connectionString))
                return func(conn);
        }

        protected void WithConnection(Action<IDbConnection> action)
        {
            using (var conn = DbConnectionFactory(_connectionString))
                action(conn);
        }
    }
}