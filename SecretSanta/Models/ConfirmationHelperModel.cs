namespace SecretSanta.Models
{
    public class ConfirmationHelperModel
    {
        public static ConfirmationHelperModel ConfirmedModel = new ConfirmationHelperModel{Confirmed = true};
        public static ConfirmationHelperModel UnconfirmedModel(long userId)=>new ConfirmationHelperModel{Confirmed = false, UserId = userId};

        private ConfirmationHelperModel()
        {
        }

        public long UserId { get; set; }
        public bool Confirmed { get; set; }
    }
}