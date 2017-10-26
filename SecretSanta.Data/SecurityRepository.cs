using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using JetBrains.Annotations;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Data
{
    public class SecurityRepository : ISantaUserStore
    {
        private readonly string _connectionString;
        
        public SecurityRepository(IConfigProvider configProvider)
        {
            _connectionString = configProvider.ConnectionString;
        }

        public void Dispose()
        {
            // Nothing?
        }

        public async Task CreateAsync([NotNull] SantaSecurityUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if(!user.IsPrivileged || !(user is SantaAdmin santaAdmin))
                throw new NotSupportedException("Creation of non-privileged users is done via standard registration form");

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.InsertAsync(santaAdmin);
            }
        }

        public async Task UpdateAsync([NotNull] SantaSecurityUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (!user.IsPrivileged || !(user is SantaAdmin santaAdmin))
                throw new NotSupportedException("Updates of non-privileged users is done via standard edit account form");

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.UpdateAsync(santaAdmin);
            }
        }

        public async Task DeleteAsync([NotNull] SantaSecurityUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (!user.IsPrivileged || !(user is SantaAdmin santaAdmin))
                throw new NotSupportedException("Deletion of non-privileged users is done via standard edit account form");

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.DeleteAsync(santaAdmin);
            }
        }

        public async Task<SantaSecurityUser> FindByIdAsync([NotNull] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException($"{nameof(userId)} cannot be null or whitespace.", nameof(userId));

            var id = SantaSecurityUser.GetId(userId, out var isAdmin);
            return await (isAdmin ? GetAdminById(id) : GetUserById(id));
        }

        private async Task<SantaSecurityUser> GetAdminById(long id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.GetAsync<SantaAdmin>(id);
            }
        }

        private async Task<SantaSecurityUser> GetUserById(long id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var santaUser = await conn.GetAsync<SantaUser>(id);

                return santaUser != null ? Mapper.Map<SantaSecurityUser>(santaUser) : null;
            }
        }

        public async Task<SantaSecurityUser> FindByNameAsync([NotNull] string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException($"{nameof(userName)} cannot be null or empty.", nameof(userName));

            using (var conn = new SqlConnection(_connectionString))
            {
                var santaAdmin= await conn.QuerySingleOrDefaultAsync<SantaAdmin>($"SELECT {nameof(SantaAdmin.AdminId)}, {nameof(SantaAdmin.UserName)}, {nameof(SantaAdmin.PasswordHash)} FROM {nameof(SantaAdmin)}s WHERE {nameof(SantaAdmin.UserName)} = @userName", new {userName});

                if (santaAdmin != null)
                    return santaAdmin;

                var santaUser = await conn.QuerySingleOrDefaultAsync<SantaUser>(
                    $"SELECT {nameof(SantaUser.Id)}, {nameof(SantaUser.DisplayName)}, {nameof(SantaUser.Email)}, {nameof(SantaUser.PasswordHash)} FROM {nameof(SantaUser)}s WHERE {nameof(SantaUser.Email)} = @userName",
                    new {userName});

                return santaUser == null ? null : Mapper.Map<SantaSecurityUser>(santaUser);
            }
        }

        public IList<SantaAdmin> GetAllAdmins()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.GetAll<SantaAdmin>().ToList();
            }
        }
    }
}