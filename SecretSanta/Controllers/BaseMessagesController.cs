using System;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public abstract class BaseMessagesController : BaseController
    {
        #region Helpers
        protected static bool IsBetween(Message message, long userId, long otherUserId)
            => message.SenderId == userId && message.RecipientId == otherUserId
               || message.SenderId == otherUserId && message.RecipientId == userId;

        protected static MessageViewModel GetMessageViewModel(Message message, long? userId) => new MessageViewModel
        {
            Id = message.Id,
            Timestamp = message.Timestamp,
            MessageText = message.MessageText,
            Label = GetLabel(message.SenderId == userId, message.SenderRole),
            OwnMessage = message.SenderId == userId
        };

        private static string GetLabel(bool fromUser, MessageRole messageSenderRole)
        {
            if (fromUser)
                return Resources.Global.Messages_FromYou;

            switch (messageSenderRole)
            {
                case MessageRole.GiftSender:
                    return Resources.Global.Messages_FromGiftor;
                case MessageRole.GiftRecipient:
                    return Resources.Global.Messages_FromAssigned;
                case MessageRole.User:
                    return Resources.Global.Messages_FromUser;
                case MessageRole.Administrator:
                    return Resources.Global.Messages_FromAdmin;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageSenderRole), messageSenderRole, null);
            }
        }


        protected struct MessageGroupingKey : IEquatable<MessageGroupingKey>
        {
            private readonly long? _senderId;
            private readonly long? _recipientId;

            public MessageGroupingKey(long? senderId, long? recipientId)
            {
                _senderId = senderId;
                _recipientId = recipientId;
            }

            public bool Equals(MessageGroupingKey other)
            {
                return (_senderId == other._senderId || _senderId == other._recipientId) &&
                       (_recipientId == other._senderId || _recipientId == other._recipientId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj) || obj.GetType() != GetType()) return false;
                return Equals((MessageGroupingKey)obj);
            }

            public override int GetHashCode()
            {
                return (_senderId + _recipientId).GetHashCode();
            }
        }
        #endregion
    }
}