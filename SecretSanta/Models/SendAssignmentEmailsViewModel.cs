using System.Collections.Generic;

namespace SecretSanta.Models
{
    public class SendAssignmentEmailsViewModel
    {
        public IList<AssignmentEmailViewModel> Assignments { get; set; }
        public IList<AbandonedEmailViewModel> Abandonments { get; set; }
    }
}