using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Models.Validation
{
    public static class PasswordValidator
    {
        private static readonly YourPasswordSucks.PasswordValidator Validator = new YourPasswordSucks.PasswordValidator();
        public static ValidationResult Validate(string password)
        {
            var result = Validator.Validate(password);
            if(result.Success)
                return ValidationResult.Success;
            return new ValidationResult(result.Message);
        }
    }
}