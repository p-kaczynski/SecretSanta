using System;
using System.ComponentModel.DataAnnotations;
using Resources;
using SecretSanta.Domain.Enums;
using SecretSanta.Models.Attributes;
using SecretSanta.Models.Validation;

namespace SecretSanta.Models
{
    public class RegistrationPostModel
    {
        [DataType(DataType.EmailAddress)]
        [HelpText(typeof(Global), "Email_HelpText")]
        [Display(Name = "Email", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(Global))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        [StringLength(254, ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }

        [DataType(DataType.Url)]
        [HelpText(typeof(Global), "FacebookURL_HelpText")]
        [Display(Name = "FacebookURL", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "FacebookURL_Required", ErrorMessageResourceType = typeof(Global))]
        [Url(ErrorMessageResourceName = "FacebookURL_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string FacebookProfileUrl { get; set; }

        [DataType(DataType.Password)]
        [HelpText(typeof(Global), "Password_HelpText")]
        [Display(Name = "Password", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        [CustomValidation(typeof(CustomValidators), "ValidatePassword", ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [HelpText(typeof(Global), "Registration_Form_Repeat_Password_HelpText")]
        [Display(Name = "Password", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Password_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string RepeatPassword { get; set; }

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

        [Display(Name = "Country", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "Country_Required", ErrorMessageResourceType = typeof(Global))]
        public CountryEntryViewModel Country { get; set; }

        [Display(Name="Registration_Form_SendAbroad", ResourceType = typeof(Global))]
        [HelpText(typeof(Global), "Registration_Form_SendAbroad_HelpText")]
        public SendAbroadOption SendAbroad { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Registration_Form_Note", ResourceType = typeof(Global))]
        [HelpText(typeof(Global), "Registration_Form_Note_HelpText")]
        public string Note { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessageResourceName = "Registration_Form_DateOfBirth_Required",
            ErrorMessageResourceType = typeof(Global))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Registration_Form_DateOfBirth", ResourceType = typeof(Global))]
        [HelpText(typeof(Global), "Registration_Form_DateOfBirth_HelpText")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Range(typeof(bool), "true", "true", ErrorMessageResourceName= "Registration_Form_Agreement_Error", ErrorMessageResourceType = typeof(Global))]
        [Display(Name = "Registration_Form_Agreement", ResourceType = typeof(Global))]
        public bool DataProtectionPolicyAgreement { get; set; }

    }
}