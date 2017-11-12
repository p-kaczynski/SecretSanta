namespace SecretSanta.Models
{
    public class AssignmentEmailViewModel
    {
        public string SenderDisplayName { get; set; }
        public string SenderEmail { get; set; }
        public string TargetDisplayName { get; set; }
        public string TargetEmail { get; set; }
        public bool Success { get; set; }
    }
}