using System.ComponentModel.DataAnnotations;
using Resources;
using SecretSanta.Models.Validation;

namespace SecretSanta.Models
{
    public class LoginModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
    }
}