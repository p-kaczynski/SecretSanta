using System.Collections.Generic;
using System.Linq;

namespace SecretSanta.Models
{
    public class UserMessagesViewModel
    {
        public bool WasAssigned { get; set; }
        public IList<MessageViewModel> WithAssigned { get; set; }
        public IList<MessageViewModel> WithGiftor { get; set; }
        public IList<MessageViewModel> WithAdmin { get; set; }
    }
}