namespace SecretSanta.Domain.Models
{
    public class Assignment
    {
        public long GiverId { get; set; }
        public long RecepientId { get; set; }
        public bool Sent { get; set; }
        public string Tracking { get; set; }
        public bool Received { get; set; }
    }
}