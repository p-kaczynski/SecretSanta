using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace SecretSanta.Models
{
    public class SettingsViewModel
    {
        [Display(Name = "RegistrationOpen", ResourceType = typeof(Global))]
        public bool RegistrationOpen { get; set; }
    }
}