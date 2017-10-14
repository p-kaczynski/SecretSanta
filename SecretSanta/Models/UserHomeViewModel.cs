namespace SecretSanta.Models
{
    public class UserHomeViewModel
    {
        public string DisplayName { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool AdminConfirmed { get; set; }
        public AssignmentViewModel Assignment { get; set; }
    }
}