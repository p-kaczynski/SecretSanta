using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        public MessageRepository(IConfigProvider configProvider) : base(configProvider)
        {
        }


        public IList<Message> GetUserMessages(long userId) => WithConnection(conn =>
            conn.Query<Message>(
                $"SELECT * FROM [{nameof(Message)}s] WHERE [{nameof(Message.SenderId)}] = @userId OR [{nameof(Message.RecipientId)}] = @userId",
                new {userId}).ToArray());

        public IList<Message> GetAdminMessages() => WithConnection(conn =>
            conn.Query<Message>(
                $"SELECT * FROM [{nameof(Message)}s] WHERE [{nameof(Message.SenderId)}] IS NULL OR [{nameof(Message.RecipientId)}] IS NULL").ToArray());

        public void SendMessageFromUserToUser(long sender, MessageRole senderRole, long recipient,
            MessageRole recipientRole,
            string messageText)
            => WithConnection(conn => conn.Execute(
                $"INSERT INTO [{nameof(Message)}s] ([{nameof(Message.SenderId)}],[{nameof(Message.RecipientId)}],[{nameof(Message.SenderRole)}],[{nameof(Message.RecipientRole)}],[{nameof(Message.MessageText)}]) " +
                $"VALUES(@{nameof(sender)}, @{nameof(recipient)}, @{nameof(senderRole)}, @{nameof(recipientRole)}, @{nameof(messageText)})",
                new {sender, recipient, senderRole, recipientRole, messageText}));

        public void SendMessageToAdmin(long sender, string messageText)
        {
            const MessageRole senderRole = MessageRole.User;
            const MessageRole recipientRole = MessageRole.Administrator;

            WithConnection(conn => conn.Execute(
                $"INSERT INTO [{nameof(Message)}s] ([{nameof(Message.SenderId)}],[{nameof(Message.SenderRole)}],[{nameof(Message.RecipientRole)}],[{nameof(Message.MessageText)}]) " +
                $"VALUES(@{nameof(sender)}, @{nameof(senderRole)}, @{nameof(recipientRole)}, @{nameof(messageText)})",
                new {sender, senderRole, recipientRole, messageText}));
        }

        public void SendMessageFromAdmin(long recipient, string messageText)
        {
            const MessageRole senderRole = MessageRole.Administrator;
            const MessageRole recipientRole = MessageRole.User;

            WithConnection(conn => conn.Execute(
                $"INSERT INTO [{nameof(Message)}s] ([{nameof(Message.RecipientId)}],[{nameof(Message.SenderRole)}],[{nameof(Message.RecipientRole)}],[{nameof(Message.MessageText)}]) " +
                $"VALUES(@{nameof(recipient)}, @{nameof(senderRole)}, @{nameof(recipientRole)}, @{nameof(messageText)})",
                new { recipient, senderRole, recipientRole, messageText }));
        }
    }
}