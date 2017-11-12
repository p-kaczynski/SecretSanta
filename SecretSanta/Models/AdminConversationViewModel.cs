using System.Collections.Generic;

namespace SecretSanta.Models
{
    public class AdminConversationViewModel
    {
        public long UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmail { get; set; }
        public string UserFacebookProfileUrl { get; set; }
        public IList<MessageViewModel> Messages { get; set; }
    }
}