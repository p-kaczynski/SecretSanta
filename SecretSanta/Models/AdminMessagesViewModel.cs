using System.Collections.Generic;

namespace SecretSanta.Models
{
    public class AdminMessagesViewModel
    {
        public IList<AdminConversationViewModel> WaitingForReply { get; set; }
        public IList<AdminConversationViewModel> Replied { get; set; }
    }
}