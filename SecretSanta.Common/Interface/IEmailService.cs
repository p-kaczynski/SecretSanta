using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IEmailService
    {
        bool SendConfirmationEmail(SantaUser user);
        bool SendPasswordResetEmail(SantaUser user);
        bool SendAssignmentEmail(SantaUser user, SantaUser target);
        bool SendAbandonmentEmail(SantaUser user, AbandonmentReason reason);
    }
}