using SecretSanta.Domain.Enums;

namespace SecretSanta.Domain.Models
{
    public class Abandoned
    {
        public long SantaUserId { get; set; }
        public AbandonmentReason Reason { get; set; }
    }
}