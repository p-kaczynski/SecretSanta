namespace SecretSanta.Common.Result
{
    public class UserEditResult : ResultBase
    {
        public bool EmailChanged { get; set; }
        public bool EmailUnavailable { get; set; }
        public bool FacebookProfileUnavailable { get; set; }
    }
}