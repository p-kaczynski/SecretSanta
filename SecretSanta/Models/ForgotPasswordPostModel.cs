using System.ComponentModel.DataAnnotations;
using Resources;

namespace SecretSanta.Models
{
    public class ForgotPasswordPostModel
    {
        [Display(Name = "Email", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(Global))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        [StringLength(254, ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string EmailAddress { get; set; }
    }
}