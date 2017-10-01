using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;
using SecretSanta.Models.Validation;

namespace SecretSanta.Models
{
    public class SantaAdminPostModel
    {
        [HiddenInput(DisplayValue = false)]
        public long AdminId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "AdminUserName", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "AdminUserName_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "AdminUserName_TooLong", ErrorMessageResourceType = typeof(Global))]
        [CustomValidation(typeof(CustomValidators), "ValidateAdminUserName", ErrorMessageResourceName = "AdminUserName_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [CustomValidation(typeof(CustomValidators), "ValidatePassword", ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Password { get; set; }
    }
}