using System;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IMessageRepository
    {
        IList<Message> GetUserMessages(long userId);
        IList<Message> GetAdminMessages();

        void SendMessageFromUserToUser(long sender, MessageRole senderRole, long recipient, MessageRole recipientRole, string messageText);
        void SendMessageToAdmin(long sender, string messageText);
        void SendMessageFromAdmin(long recipient, string messageText);

    }
}