using SecretSanta.Common.Interface;
using SecretSanta.Domain.Enums;

namespace SecretSanta.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IEmailService _emailService;

        private readonly bool _sendEmail;

        public MessageService(IUserRepository userRepository, IEmailService emailService, IMessageRepository messageRepository, IConfigProvider configProvider)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _messageRepository = messageRepository;
            _sendEmail = configProvider.SendMessageEmails;
        }

        public void SendMessageFromUserToUser(long sender, MessageRole senderRole, long recipient, MessageRole recipientRole,
            string messageText)
        {
            // Save to db
            _messageRepository.SendMessageFromUserToUser(sender,senderRole,recipient,recipientRole,messageText);

            // send email
            if (_sendEmail)
                _emailService.SendNewMessageNotification(_userRepository.GetUserWithoutProtectedData(recipient),
                    senderRole, messageText);
        }

        public void SendMessageToAdmin(long sender, string messageText)
        {
            // Save to db
            _messageRepository.SendMessageToAdmin(sender, messageText);

            // send email
            if (_sendEmail)
                _emailService.SendNewAdminSupportMessageNotification(_userRepository.GetUserWithoutProtectedData(sender), messageText);
        }

        public void SendMessageFromAdmin(long recipient, string messageText)
        {
            // Save to db
            _messageRepository.SendMessageFromAdmin(recipient, messageText);

            // send email
            if (_sendEmail)
                _emailService.SendNewMessageNotification(_userRepository.GetUserWithoutProtectedData(recipient), MessageRole.Administrator, messageText);
        }
    }
}