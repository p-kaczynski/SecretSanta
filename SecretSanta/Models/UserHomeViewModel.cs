namespace SecretSanta.Models
{
    public class UserHomeViewModel
    {
        public string DisplayName { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool AdminConfirmed { get; set; }
        public AssignmentViewModel Assignment { get; set; }
        public bool InboundGiftEnRoute { get; set; }
        public bool InboundGiftArrived { get; set; }
        public bool OutboundGiftEnRoute { get; set; }
        public bool OutboundGiftArrived { get; set; }
        public bool AssignmentPerformed { get; set; }
        public bool InboundGiftMissing { get; set; }
        public bool OutboundGiftMissing { get; set; }
    }
}