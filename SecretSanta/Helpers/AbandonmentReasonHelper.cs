using SecretSanta.Domain.Enums;

namespace SecretSanta.Helpers
{
    public static class AbandonmentReasonHelper
    {
        public static string GetUserFriendlyDescription(this AbandonmentReason reason)
        {
            return Resources.Global.ResourceManager.GetString($"AbandonmentReason_Text_{reason.ToString()}") ??
                   Resources.Global.AbandonmentReason_Unknown;
        }
    }
}