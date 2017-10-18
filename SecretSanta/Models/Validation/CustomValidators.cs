using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using YourPasswordSucks;

namespace SecretSanta.Models.Validation
{
    public static class CustomValidators
    {
        public static ValidationResult ValidatePassword(string password)
        {
            var validator = DependencyResolver.Current.GetService<PasswordValidator>();
            var result = validator.Validate(password);
            if(result.Success)
                return ValidationResult.Success;
            return new ValidationResult(result.Message);
        }

        public static ValidationResult ValidateAdminUserName(string username)
            => username != null && username.All(char.IsLetterOrDigit)
                ? ValidationResult.Success
                : new ValidationResult("The admin user name must be composed of letters or digits");
    }
}