﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using JetBrains.Annotations;
using SecretSanta.Common.Helpers;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Common.Result;

namespace SecretSanta.Data
{
    public class UserRepository : IUserRepository
    {
        private static readonly Random Random = new Random();
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IAssignmentAlgorithm _algorithm;
        private readonly string _connectionString;

        public Func<string, IDbConnection> DbConnectionFactory { get; set; } = connectionString =>  new SqlConnection(connectionString);

        public UserRepository(IConfigProvider configProvider, IEncryptionProvider encryptionProvider, IAssignmentAlgorithm algorithm)
        {
            _encryptionProvider = encryptionProvider;
            _algorithm = algorithm;
            _connectionString = configProvider.ConnectionString;
        }

        public long InsertUser([NotNull] SantaUser user)
        {
            _encryptionProvider.Encrypt(user);
            return WithConnection(conn => conn.Insert(user));
        }

        public UserEditResult UpdateUser([NotNull] SantaUser updateUser)
        {
            var emailChanged = !updateUser.Email.Equals(
                WithConnection(conn => conn.Get<SantaUser>(updateUser.Id)).Email, StringComparison.OrdinalIgnoreCase);

            if(emailChanged && !CheckEmail(updateUser.Email))
                    return new UserEditResult{EmailChanged = true, EmailUnavailable = true, Success = false};

            _encryptionProvider.Encrypt(updateUser);

            WithConnection(conn =>
                    conn.Execute($"UPDATE [dbo].[{nameof(SantaUser)}s]   SET " +
                                 "[Email] = @Email " +
                                 "     ,[FacebookProfileUrl] = @FacebookProfileUrl" +
                                 "      ,[DisplayName] = @DisplayName" +
                                 "      ,[FullName] = @FullName" +
                                 "      ,[AddressLine1] = @AddressLine1" +
                                 "      ,[AddressLine2] = @AddressLine2" +
                                 "      ,[PostalCode] = @PostalCode" +
                                 "      ,[City] = @City" +
                                 "      ,[Country] = @Country" +
                                 "      ,[SendAbroad] = @SendAbroad" +
                                 "      ,[Note] = @Note " +
                                 "      ,[AdminConfirmed] = 0 " +
                                 "WHERE [Id] = @Id",
                new
                {
                    updateUser.Id,
                    updateUser.Email,
                    updateUser.FacebookProfileUrl,
                    updateUser.DisplayName,
                    updateUser.FullName,
                    updateUser.AddressLine1,
                    updateUser.AddressLine2,
                    updateUser.PostalCode,
                    updateUser.City,
                    updateUser.Country,
                    updateUser.SendAbroad,
                    updateUser.Note
                }));


            if (emailChanged)
                WithConnection(conn =>
                    conn.Execute($"UPDATE [dbo].[{nameof(SantaUser)}s]   SET [EmailConfirmed] = 0"));

            return new UserEditResult{Success = true, EmailChanged = emailChanged};
        }

        public void SetPassword(PasswordResetModel model) 
            => WithConnection(conn =>
            conn.Execute($"UPDATE {nameof(SantaUser)}s SET PasswordHash = @passwordBytes WHERE Id = @userId",
                new {model.UserId, model.PasswordBytes}));


        public bool CheckEmail(string email) 
            => WithConnection(conn =>conn.QuerySingleOrDefault<SantaUser>(
                                     $"SELECT Email FROM {nameof(SantaUser)}s WHERE Email = @email", new {email})) == null;

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

        public SantaUser GetUserWithoutProtectedDataByEmail([NotNull]string emailAddress)
        {
            var model = WithConnection(conn => conn.QuerySingleOrDefault<SantaUser>($"SELECT * FROM {nameof(SantaUser)}s WHERE Email = @emailAddress", new {emailAddress}));
            if (model == null)
                return null;

            model.ClearDataProtected();
            return model;
        }

        public IList<SantaUser> GetAllUsersWithoutProtectedData()
        {
            var models = WithConnection(conn => conn.GetAll<SantaUser>().ToList());
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

        public void EmailConfirm(long id)
        {
            WithConnection(conn =>
            {
                var model = conn.Get<SantaUser>(id);
                model.EmailConfirmed = true;
                conn.Update(model);
            });
        }

        public void DeleteUser(long id)
        {
            WithConnection(conn =>
            {
                var model = conn.Get<SantaUser>(id);
                conn.Delete(model);
            });
        }

        public bool WasAssigned()
        {
            return WithConnection(conn => conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Assignments") > 0);
        }

        public AssignmentResult AssignRecipients()
        {

            // prepare predicate
            bool ConfirmedOnly(SantaUser user) => user.AdminConfirmed && user.EmailConfirmed;

            // get all users
            var allUsers = GetAllUsersWithoutProtectedData().Where(ConfirmedOnly).ToArray();

            if (!allUsers.Any())
                throw new InvalidOperationException("No users have signed up!");

            var result =_algorithm.Assign(allUsers);

            // verify algorithm
            _algorithm.Verify(result); //this will throw for failures

            // we can now execute massive amount of inserts ;)
            WithConnection(conn =>
            {
                foreach (var assignment in result.Assignments)
                    conn.Execute(@"INSERT INTO [Assignments] (GiverId, RecepientId) VALUES(@GiverId, @RecepientId)",
                        new { assignment.GiverId, assignment.RecepientId });
            });

            // and save abandoned for future review
            WithConnection(conn =>
            {
                foreach (var abandonedUser in result.Abandoned)
                    conn.Execute(@"INSERT INTO [Abandoned] (SantaUserId, Reason) VALUES(@SantaUserId, @Reason)",
                        new { abandonedUser.SantaUserId, abandonedUser.Reason });
            });

            // verify
            var assignmentsFromDb =
                WithConnection(conn => conn.ExecuteScalar<int>("SELECT COUNT(*) FROM [Assignments]"));

            if (assignmentsFromDb != result.Assignments.Count)
                throw new InvalidOperationException("Inserts were performed, but there is numerical mismatch!");

            // set lookup for frontend
            result.UserDisplayById = allUsers.ToDictionary(user => user.Id, user => user);
            return result;
        }

        public long? GetAssignedPartnerIdForUser(long id)
        {
            return WithConnection(
                conn => conn.QuerySingleOrDefault<long?>(@"SELECT RecepientId FROM Assignments WHERE GiverId = @id",
                new {id}));
        }

        public AssignmentResult GetAssignments()
        {
            // get all users
            var allUsers = GetAllUsersWithoutProtectedData();

            // get all assignments
            var allAssignments = WithConnection(conn => conn.Query<Assignment>("SELECT * FROM [Assignments]")).ToArray();

            // get all abandoned
            var allAbandoned = WithConnection(conn => conn.Query<Abandoned>("SELECT * FROM [Abandoned]")).ToArray();

            return new AssignmentResult{Assignments = allAssignments, UserDisplayById = allUsers.ToDictionary(user=>user.Id, user=>user), Abandoned = allAbandoned};
        }

        private T WithConnection<T>(Func<IDbConnection, T> func)
        {
            using (var conn = DbConnectionFactory(_connectionString))
                return func(conn);
        }

        private void WithConnection(Action<IDbConnection> action)
        {
            using (var conn = DbConnectionFactory(_connectionString))
                action(conn);
        }
    }
}