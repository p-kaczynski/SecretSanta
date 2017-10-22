using System.ComponentModel.DataAnnotations;
using Resources;

namespace SecretSanta.Models
{
    public class SantaUserViewModel : SantaUserPostModel
    {
        [Display(Name= "User_View_EmailConfirmed", ResourceType = typeof(Global))]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "User_View_AdminConfirmed", ResourceType = typeof(Global))]
        public bool AdminConfirmed { get; set; }
    }
}