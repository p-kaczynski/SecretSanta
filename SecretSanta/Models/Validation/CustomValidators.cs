using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SecretSanta.Models.Validation
{
    public static class CustomValidators
    {
        private static readonly YourPasswordSucks.PasswordValidator Validator = new YourPasswordSucks.PasswordValidator();
        public static ValidationResult ValidatePassword(string password)
        {
            var result = Validator.Validate(password);
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