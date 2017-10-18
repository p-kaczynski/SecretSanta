namespace SecretSanta.Domain.Models.Extensions
{
    public static class SantaUserExtensions
    {
        public static string GetEmailConfirmationTokenGenerationInputString(this SantaUser user) =>
            $"{user.Id}:{user.Email}";

        public static string GetPasswordResetTokenGenerationInputString(this SantaUser user) =>
            $"password reset for user {user.Id}";
    }
}