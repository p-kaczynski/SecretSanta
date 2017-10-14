using System.ComponentModel.DataAnnotations;
using Resources;

namespace SecretSanta.Models
{
    public class LoginModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "LoginName", ResourceType = typeof(Global))]
        [Required]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
    }
}