using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IEmailService
    {
        bool SendConfirmationEmail(SantaUser user);
        void SendAssignmentEmail(SantaUser user, SantaUser target);
        void SendAbandonmentEmail(SantaUser user);
    }
}