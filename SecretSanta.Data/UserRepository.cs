using System;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly string _connectionString;
        public UserRepository(IConfigProvider configProvider, IEncryptionProvider encryptionProvider)
        {
            _encryptionProvider = encryptionProvider;
            _connectionString = configProvider.ConnectionString;
        }

        public long InsertUser(SantaUser user)
        {
            _encryptionProvider.Encrypt(user);
            return WithConnection(conn => conn.Insert(user));
        }

        public SantaUser GetUser(long id)
        {
            var model = WithConnection(conn => conn.Get<SantaUser>(id));
            _encryptionProvider.Decrypt(model);
            return model;
        }

        private T WithConnection<T>(Func<SqlConnection, T> func)
        {
            using (var conn = new SqlConnection(_connectionString))
                return func(conn);
        }

        private void WithConnection(Action<SqlConnection> action)
        {
            using (var conn = new SqlConnection(_connectionString))
                action(conn);
        }
    }
}