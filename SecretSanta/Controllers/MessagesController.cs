using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Enums;
using SecretSanta.Helpers;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    // TODO: This is candidate for full rewrite. Leaving in working but crappy state because of time constraints.
    [Authorize(Roles = SantaUserManager.UserRole)]
    public class MessagesController : BaseMessagesController
    {
        private readonly IMessageReadOnlyRepository _messageRepository;
        private readonly IMessageService _messageService;
        private readonly IUserRepository _userRepository;

        public MessagesController(IMessageReadOnlyRepository messageRepository, IUserRepository userRepository, IMessageService messageService)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _messageService = messageService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return RedirectToAction("Index", "Home");

            var userMessages = _messageRepository.GetUserMessages(userId.Value);
            var model = new UserMessagesViewModel
            {
                WasAssigned = _userRepository.WasAssigned(),
                WithAssigned  = new List<MessageViewModel>(),
                WithGiftor = new List<MessageViewModel>(),
                WithAdmin = new List<MessageViewModel>()
            };

            foreach (var message in userMessages.OrderBy(m => m.Timestamp))
            {
                if (message.SenderId == userId)
                {
                    // From user, see in what role:
                    switch (message.SenderRole)
                    {
                        // for user messages, he can be User (with admin), Sender (with assigned) or Recipient (with his santa)
                        case MessageRole.User:
                            model.WithAdmin.Add(GetMessageViewModel(message, userId));
                            break;
                        case MessageRole.GiftSender:
                            model.WithAssigned.Add(GetMessageViewModel(message, userId));
                            break;
                        case MessageRole.GiftRecipient:
                            model.WithGiftor.Add(GetMessageViewModel(message, userId));
                            break;
                    }
                }
                else
                {
                    // from someone else, see in what role:
                    switch (message.SenderRole)
                    {
                        // for incoming messages, he can be Admin (with admin), Sender (with Santa) or Recipient (with assigned)
                        case MessageRole.Administrator:
                            model.WithAdmin.Add(GetMessageViewModel(message, userId));
                            break;
                        case MessageRole.GiftSender:
                            model.WithGiftor.Add(GetMessageViewModel(message, userId));
                            break;
                        case MessageRole.GiftRecipient:
                            model.WithAssigned.Add(GetMessageViewModel(message, userId));
                            break;
                    }
                }
            }

            return View(model);
        }



        [HttpGet]
        public ActionResult PostConversation(MessageRole sender, MessageRole recipient)
        {
            return View(new ConversationPostModel {SenderRole = sender, RecipientRole = recipient});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostConversation(ConversationPostModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetUserId();
            if (!userId.HasValue)
                return RedirectToAction("Index", "Home");

            if(model.RecipientRole == MessageRole.Administrator)
                _messageService.SendMessageToAdmin(userId.Value, model.MessageText);
            else
                _messageService.SendMessageFromUserToUser(userId.Value, model.SenderRole, GetRecipient(model.RecipientRole, userId.Value), model.RecipientRole, model.MessageText);


            return RedirectToAction("Index");
        }

        private long GetRecipient(MessageRole modelRecipientRole, long userId)
        {
            switch (modelRecipientRole)
            {
                case MessageRole.GiftRecipient:
                    return _userRepository.GetAssignedPartnerIdForUser(userId) ??
                           throw new InvalidOperationException(
                               $"User ({userId}) that has not been in assignment tried to send a message to recipient");
                case MessageRole.GiftSender:
                    return _userRepository.GetUserAssignedTo(userId) ?? throw new InvalidOperationException(
                               $"User ({userId}) that has not been in assignment tried to send a message to recipient");
            }
            throw new ArgumentOutOfRangeException(nameof(modelRecipientRole),$"User ({userId}) requested recipient for role {modelRecipientRole.ToString()} which should not have happened");
        }
    }
}