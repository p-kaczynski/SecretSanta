using System;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;

namespace SecretSanta.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtp = new SmtpClient();
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly TimeSpan _resendConfirmationCooldown;

        public EmailService(IEncryptionProvider encryptionProvider, IConfigProvider configProvider)
        {
            _encryptionProvider = encryptionProvider;
            _resendConfirmationCooldown = configProvider.ResendConfirmationCooldown;
        }


        public bool SendConfirmationEmail(SantaUser user)
        {
            const string ddosPreventionKeyPrefix = "santa.user.confirmation_resend_for_";
            // prevent abuse:
            var cacheKey = ddosPreventionKeyPrefix + user.Email;
            if (HttpRuntime.Cache.Get(cacheKey) != null)
                return false;


            var token = _encryptionProvider.GetEmailVerificationToken(user);

            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var link = urlHelper.Action("Confirm", "Account", new {userId = user.Id, token}, HttpContext.Current.Request.Url.Scheme);

            var body = string.Format(Resources.Global.Email_Verification_Body, user.DisplayName, link);

            if (!SendEmail(user.Email, Resources.Global.Email_Verification_Subject, body)) return false;
            
            // save for ddos prevention
            HttpRuntime.Cache.Add(cacheKey, true, null, DateTime.Now + _resendConfirmationCooldown,
                Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

            return true;
        }

        public void SendAssignmentEmail(SantaUser user, SantaUser target)
        {
            throw new System.NotImplementedException();
        }

        public void SendAbandonmentEmail(SantaUser user)
        {
            throw new System.NotImplementedException();
        }

        private bool SendEmail(string to, string subject, string body)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(to);
                message.Subject = subject;
                message.Body = body;

                try
                {
                    _smtp.Send(message);
                    return true;
                }
                catch (SmtpException smptException)
                {
                    // TODO: Log
                    return false;
                }
            }
        }
    }
}