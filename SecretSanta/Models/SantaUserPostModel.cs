using System.ComponentModel.DataAnnotations;
using Resources;
using SecretSanta.Models.Validation;

namespace SecretSanta.Models
{
    public class SantaUserPostModel
    {
        [Display(Name = "Email", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(Global))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [CustomValidation(typeof(PasswordValidator),"Validate", ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Password { get; set; }

        [Display(Name = "DisplayName", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "DisplayName_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "DisplayName_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string DisplayName { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "FullName_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "FullName_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string FullName { get; set; }

        [Display(Name = "AddressLine1", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "AddressLine1_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "AddressLine1_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string AddressLine1 { get; set; }

        [Display(Name = "AddressLine2", ResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "AddressLine2_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string AddressLine2 { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "PostalCode_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(32, ErrorMessageResourceName = "PostalCode_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string PostalCode { get; set; }

        [Display(Name = "City", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "City_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "City_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string City { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Country_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "Country_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string Country { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Global))]
        public string Note { get; set; }

    }
}