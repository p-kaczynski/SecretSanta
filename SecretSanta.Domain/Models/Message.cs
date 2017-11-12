using System;
using SecretSanta.Domain.Enums;

namespace SecretSanta.Domain.Models
{
    public sealed class Message
    {
        public long Id { get; set; }
        public long? SenderId { get; set; }
        public long? RecipientId { get; set; }
        public MessageRole SenderRole { get; set; }
        public MessageRole RecipientRole { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}