using System.ComponentModel.DataAnnotations;
using Resources;
using SecretSanta.Models.Validation;

namespace SecretSanta.Models
{
    public class SantaUserPostModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(Global))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        [StringLength(254, ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "FacebookURL", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "FacebookURL_Required", ErrorMessageResourceType = typeof(Global))]
        [Url(ErrorMessageResourceName = "FacebookURL_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string FacebookProfileUrl { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Global))]
        [CustomValidation(typeof(CustomValidators), "ValidatePassword", ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Password { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "DisplayName", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "DisplayName_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "DisplayName_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string DisplayName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "FullName", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "FullName_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "FullName_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string FullName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "AddressLine1", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "AddressLine1_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "AddressLine1_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string AddressLine1 { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "AddressLine2", ResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "AddressLine2_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string AddressLine2 { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "PostalCode", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "PostalCode_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(32, ErrorMessageResourceName = "PostalCode_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string PostalCode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "City_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "City_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string City { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Country", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Country_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(200, ErrorMessageResourceName = "Country_TooLong", ErrorMessageResourceType = typeof(Global))]
        public string Country { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Note", ResourceType = typeof(Global))]
        public string Note { get; set; }

    }
}