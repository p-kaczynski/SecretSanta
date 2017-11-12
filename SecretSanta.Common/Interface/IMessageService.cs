using SecretSanta.Domain.Enums;

namespace SecretSanta.Common.Interface
{
    public interface IMessageService
    {
        void SendMessageFromUserToUser(long sender, MessageRole senderRole, long recipient, MessageRole recipientRole, string messageText);
        void SendMessageToAdmin(long sender, string messageText);
        void SendMessageFromAdmin(long recipient, string messageText);
    }
}