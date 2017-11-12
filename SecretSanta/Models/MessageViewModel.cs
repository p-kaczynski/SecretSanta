using System;

namespace SecretSanta.Models
{
    public class MessageViewModel
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public bool OwnMessage { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}