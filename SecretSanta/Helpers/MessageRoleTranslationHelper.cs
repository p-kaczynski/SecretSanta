using System;
using SecretSanta.Domain.Enums;

namespace SecretSanta.Helpers
{
    public static class MessageRoleTranslationHelper
    {
        public static string From(MessageRole role)
        {
            switch (role)
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
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }

        public static string To(MessageRole role)
        {
            switch (role)
            {
                case MessageRole.GiftSender:
                    return Resources.Global.Messages_ToGiver;
                case MessageRole.GiftRecipient:
                    return Resources.Global.Messages_ToAssigned;
                case MessageRole.Administrator:
                    return Resources.Global.Messages_ToAdmin;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }
    }
}