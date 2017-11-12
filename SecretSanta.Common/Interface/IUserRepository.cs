using System.Collections.Generic;
using SecretSanta.Common.Result;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Common.Interface
{
    public interface IUserRepository
    {
        SantaUser GetUser(long id);
        long InsertUser(SantaUser user);
        UserEditResult UpdateUser(SantaUser updateUser);
        void SetPassword(PasswordResetModel model);

        SantaUser GetUserWithoutProtectedData(long id);
        SantaUser GetUserWithoutProtectedDataByEmail(string emailAddress);
        IList<SantaUser> GetAllUsersWithoutProtectedData();

        void AdminConfirm(long id);
        void EmailConfirm(long id);

        void DeleteUser(long id);

        bool CheckEmail(string email);
        bool CheckFacebookProfileUri(string fbUri);

        long? GetAssignedPartnerIdForUser(long id);
        long? GetUserAssignedTo(long id);

        bool WasAssigned();

        AssignmentResult AssignRecipients();

        Assignment GetOutboundAssignment(long userId);

        Assignment GetInboundAssignment(long userId);

        AssignmentResult GetAssignments();

        void SetGiftSent(long userId, string tracking);
        void SetGiftReceived(long userId);
    }
}