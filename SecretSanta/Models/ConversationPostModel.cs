using System;
using System.ComponentModel.DataAnnotations;
using Resources;
using SecretSanta.Domain.Enums;
using SecretSanta.Helpers;
using SecretSanta.Models.Attributes;

namespace SecretSanta.Models
{
    public class ConversationPostModel
    {
        public MessageRole SenderRole { get; set; }
        public MessageRole RecipientRole { get; set; }

        [DataType(DataType.MultilineText)]
        [HelpText(typeof(Global), "ConversationPostModel_MessageText_HelpText")]
        [Display(Name = "ConversationPostModel_MessageText", ResourceType = typeof(Global))]
        [Required(ErrorMessageResourceName = "ConversationPostModel_MessageText_Required", ErrorMessageResourceType = typeof(Global))]
        [StringLength(2048, ErrorMessageResourceName = "ConversationPostModel_MessageText_Invalid", ErrorMessageResourceType = typeof(Global))]
        public string MessageText { get; set; }
    }
}