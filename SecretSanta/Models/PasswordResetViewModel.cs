using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;
using SecretSanta.Models.Validation;

namespace SecretSanta.Models
{
    public class PasswordResetViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public long UserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "PasswordReset_Token_Display", ResourceType = typeof(Global))]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Global))]
        [CustomValidation(typeof(CustomValidators), "ValidatePassword", ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Global))]
        [CustomValidation(typeof(CustomValidators), "ValidatePassword", ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string ConfirmNewPassword { get; set; }

    }
}