using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper.Contrib.Extensions;
using SecretSanta.Common.Helpers;
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

        public bool UpdateUser(SantaUser user)
        {
            _encryptionProvider.Encrypt(user);
            return WithConnection(conn => conn.Update(user));
        }

        public SantaUser GetUser(long id)
        {
            var model = WithConnection(conn => conn.Get<SantaUser>(id));
            if (model == null)
                return null;

            _encryptionProvider.Decrypt(model);
            return model;
        }

        public SantaUser GetUserWithoutProtectedData(long id)
        {
            var model = WithConnection(conn => conn.Get<SantaUser>(id));
            if (model == null)
                return null;

            model.ClearDataProtected();
            return model;
        }

        public IList<SantaUser> GetAllUsersWithoutProtectedData()
        {
            var models = WithConnection(conn => conn.GetAll<SantaUser>().ToArray());
            foreach(var model in models)
                model.ClearDataProtected();

            return models;
        }

        public void AdminConfirm(long id)
        {
            WithConnection(conn =>
            {
                var model = conn.Get<SantaUser>(id);
                model.AdminConfirmed = true;
                conn.Update(model);
            });
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