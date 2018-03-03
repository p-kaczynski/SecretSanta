using System;
using System.Collections.Generic;
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
    public class UserRepository : BaseRepository, IUserRepository
    {
        private static readonly Random Random = new Random();
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IAssignmentAlgorithm _algorithm;

        public UserRepository(IConfigProvider configProvider, IEncryptionProvider encryptionProvider, IAssignmentAlgorithm algorithm) : base(configProvider)
        {
            _encryptionProvider = encryptionProvider;
            _algorithm = algorithm;
        }

        public long InsertUser([NotNull] SantaUser user)
        {
            _encryptionProvider.Encrypt(user);
            return WithConnection(conn => conn.Insert(user));
        }

        public UserEditResult UpdateUser([NotNull] SantaUser updateUser)
        {
            var current = WithConnection(conn => conn.Get<SantaUser>(updateUser.Id));

            var emailChanged = !updateUser.Email.Equals(current.Email, StringComparison.OrdinalIgnoreCase);
            var fbProfileChanged =
                !updateUser.FacebookProfileUrl.Equals(current.FacebookProfileUrl, StringComparison.OrdinalIgnoreCase);

            if(emailChanged && !CheckEmail(updateUser.Email))
                    return new UserEditResult{ EmailUnavailable = true, Success = false};

            if(fbProfileChanged && !CheckFacebookProfileUri(updateUser.FacebookProfileUrl))
                return new UserEditResult { FacebookProfileUnavailable = true, Success = false };

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
                    conn.Execute($"UPDATE [dbo].[{nameof(SantaUser)}s]   SET [EmailConfirmed] = 0 WHERE [Id] = @Id", new{ updateUser.Id }));

            return new UserEditResult{Success = true, EmailChanged = emailChanged};
        }

        public void SetPassword(PasswordResetModel model) 
            => WithConnection(conn =>
            conn.Execute($"UPDATE {nameof(SantaUser)}s SET PasswordHash = @passwordBytes WHERE Id = @userId",
                new {model.UserId, model.PasswordBytes}));


        public bool CheckEmail(string email) 
            => WithConnection(conn =>conn.QuerySingleOrDefault<SantaUser>(
                                     $"SELECT Email FROM {nameof(SantaUser)}s WHERE Email = @email", new {email})) == null;

        public bool CheckFacebookProfileUri(string fbUri)
            => WithConnection(conn => conn.QuerySingleOrDefault<SantaUser>(
                   $"SELECT FacebookProfileUrl FROM {nameof(SantaUser)}s WHERE FacebookProfileUrl = @fbUri", new { fbUri })) == null;

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

        public IList<SantaUser> GetAllUsers() => WithConnection(conn => conn.GetAll<SantaUser>()).Select(model =>
            {
                _encryptionProvider.Decrypt(model);
                return model;
            }).ToList();

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
            return WithConnection(conn => conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM [{nameof(Assignment)}s]") > 0);
        }

        public void AssignRecipients(AssignmentResult result)
        {
            // we can now execute massive amount of inserts ;)
            WithConnection(conn =>
            {
                foreach (var assignment in result.Assignments)
                    conn.Execute($@"INSERT INTO [{nameof(Assignment)}s] (GiverId, RecepientId) VALUES(@GiverId, @RecepientId)",
                        new { assignment.GiverId, assignment.RecepientId });
            });

            // and save abandoned for future review
            WithConnection(conn =>
            {
                foreach (var abandonedUser in result.Abandoned)
                    conn.Execute($@"INSERT INTO [{nameof(Abandoned)}] (SantaUserId, Reason) VALUES(@SantaUserId, @Reason)",
                        new { abandonedUser.SantaUserId, abandonedUser.Reason });
            });

            // verify
            var assignmentsFromDb =
                WithConnection(conn => conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM [{nameof(Assignment)}s]"));

            if (assignmentsFromDb != result.Assignments.Count)
                throw new InvalidOperationException("Inserts were performed, but there is numerical mismatch!");
        }

        public long? GetAssignedPartnerIdForUser(long id) => WithConnection(
            conn => conn.QuerySingleOrDefault<long?>(
                $@"SELECT RecepientId FROM [{nameof(Assignment)}s] WHERE GiverId = @id",
                new {id}));

        public long? GetUserAssignedTo(long id) => WithConnection(
            conn => conn.QuerySingleOrDefault<long?>(
                $@"SELECT GiverId FROM [{nameof(Assignment)}s] WHERE RecepientId = @id",
                new {id}));


        public AssignmentResult GetAssignments()
        {
            // get all users
            var allUsers = GetAllUsers();

            // get all assignments
            var allAssignments = WithConnection(conn => conn.Query<Assignment>($"SELECT * FROM [{nameof(Assignment)}s]")).ToArray();

            // get all abandoned
            var allAbandoned = WithConnection(conn => conn.Query<Abandoned>($"SELECT * FROM [{nameof(Abandoned)}]")).ToArray();

            return new AssignmentResult{Assignments = allAssignments, UserDisplayById = allUsers.ToDictionary(user=>user.Id, user=>user), Abandoned = allAbandoned};
        }

        public void SetGiftSent(long userId, string tracking)
        {
            WithConnection(conn =>
                conn.Execute(
                    $"UPDATE [dbo].[{nameof(Assignment)}s] SET Sent = 1, Tracking = @tracking WHERE GiverId = @userId",
                    new {userId, tracking}));
        }

        public Assignment GetOutboundAssignment(long userId)
        {
            return WithConnection(conn =>
                conn.QuerySingle<Assignment>($"SELECT * FROM [{nameof(Assignment)}s] WHERE GiverId = @userId", new {userId}));
        }

        public Assignment GetInboundAssignment(long userId)
        {
            return WithConnection(conn =>
                conn.QuerySingle<Assignment>($"SELECT * FROM [{nameof(Assignment)}s] WHERE RecepientId = @userId", new { userId }));
        }

        public void SetGiftReceived(long userId)
        {
            WithConnection(conn =>
                conn.Execute(
                    $"UPDATE [dbo].[{nameof(Assignment)}s] SET Received = 1, Missing = 0 WHERE RecepientId = @userId",
                    new { userId }));
        }

        public bool SetGiftMissing(long userId)
        {
            var assignment = GetInboundAssignment(userId);
            if (assignment.Received)
                return false;

            WithConnection(conn =>
                conn.Execute(
                    $"UPDATE [dbo].[{nameof(Assignment)}s] SET Missing = 1 WHERE RecepientId = @userId",
                    new { userId }));

            return true;
        }
    }
}