namespace SecretSanta.Domain.SecurityModels
{
    public class PasswordResetModel
    {
        public long UserId { get; set; }
        public byte[] PasswordBytes { get; set; }
    }
}