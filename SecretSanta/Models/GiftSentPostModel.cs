using System.ComponentModel.DataAnnotations;
using Resources;
using SecretSanta.Models.Attributes;

namespace SecretSanta.Models
{
    public class GiftSentPostModel
    {
        [HelpText(typeof(Global), "GiftSent_Form_Confirmation_HelpText")]
        [Display(Name = "GiftSent_Form_Confirmation", ResourceType = typeof(Global))]
        [Range(typeof(bool), "true", "true", ErrorMessageResourceName = "GiftSent_MustConfirm", ErrorMessageResourceType = typeof(Global))]
        public bool Confirmation { get; set; }

        [DataType(DataType.Text)]
        [HelpText(typeof(Global), "GiftSent_Form_Tracking_HelpText")]
        [Display(Name = "GiftSent_Form_Tracking", ResourceType = typeof(Global))]
        public string Tracking { get; set; }
    }
}