using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Helpers;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles = SantaUserManager.AdminRole)]
    public class AdminMessageController : BaseMessagesController
    {
        private readonly IMessageService _messageService;
        private readonly IMessageReadOnlyRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public AdminMessageController(IMessageReadOnlyRepository messageRepository, IUserRepository userRepository, IMessageService messageService)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _messageService = messageService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var adminMessages = _messageRepository.GetAdminMessages();

            var users = _userRepository.GetAllUsersWithoutProtectedData().ToDictionary(u => u.Id, u => u);

            var model = new AdminMessagesViewModel
            {
                Replied = new List<AdminConversationViewModel>(),
                WaitingForReply = new List<AdminConversationViewModel>()
            };

            foreach (var conversation in adminMessages.GroupBy(m=>new MessageGroupingKey(m.SenderId,m.RecipientId)))
            {
                var userId = conversation.First().SenderId ?? conversation.First().RecipientId;
                if (!userId.HasValue)
                    continue; // this should not happen, but to satisfy code analysis...

                var user = users[userId.Value];

                var conversationModel = new AdminConversationViewModel
                {
                    UserId = user.Id,
                    UserDisplayName = user.DisplayName,
                    UserFacebookProfileUrl = user.FacebookProfileUrl,
                    UserEmail = user.Email,
                    Messages = conversation.Select(m=>GetMessageViewModel(m,null)).OrderBy(m=>m.Timestamp).ToArray()
                };
                if(conversation.OrderByDescending(m=>m.Timestamp).First().RecipientId.HasValue)
                    model.Replied.Add(conversationModel); // newest has recipient Id -> it went to user
                else
                    model.WaitingForReply.Add(conversationModel); // newest does not have recipient Id -> it went to Admin
            }
            

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostReply(AdminReplyModel model)
        {
            _messageService.SendMessageFromAdmin(model.UserId,model.MessageText);
            return RedirectToAction("Index");
        }
    }
}