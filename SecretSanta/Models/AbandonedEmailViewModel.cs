using SecretSanta.Domain.Enums;

namespace SecretSanta.Models
{
    public class AbandonedEmailViewModel
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public AbandonmentReason Reason { get; set; }
        public bool Success { get; set; }
    }
}