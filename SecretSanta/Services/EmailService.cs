using System;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using Autofac.Features.Indexed;
using NLog;
using SaturnV;
using SecretSanta.Common;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.Models.Extensions;

namespace SecretSanta.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfigProvider _configProvider;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly SmtpClient _smtp;
        private readonly SecureAccessTokenSource _emailConfirmationTokenSource;
        private readonly SecureAccessTokenSource _passwordResetTokenSource;
        private readonly MailgunService _mailgun;

        public EmailService(IConfigProvider configProvider, IIndex<TokenSourceType,SecureAccessTokenSource> satIndex)
        {
            _configProvider = configProvider;
            _emailConfirmationTokenSource = satIndex[TokenSourceType.EmailConfirmation];
            _passwordResetTokenSource = satIndex[TokenSourceType.PasswordReset];

            if (_configProvider.UseMailgun)
                _mailgun = new MailgunService(_configProvider);
            else
                _smtp = new SmtpClient();
        }

        public bool SendConfirmationEmail(SantaUser user)
        {
            const string ddosPreventionKeyPrefix = "santa.user.confirmation_resend_for_";
            // prevent abuse:
            var cacheKey = ddosPreventionKeyPrefix + user.Id;
            using (var abuse = new EmailAbuseProtection(cacheKey, _configProvider.ResendConfirmationCooldown))
            {
                var token = _emailConfirmationTokenSource.GetAccessCodeFor(
                    user.GetEmailConfirmationTokenGenerationInputString());

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var link = urlHelper.Action("Confirm", "Account", new {userId = user.Id, token},
                    HttpContext.Current.Request.Url.Scheme);

                var body = string.Format(Resources.Global.Email_Verification_Body, user.DisplayName, link);

                if (!SendEmail(user.Email, Resources.Global.Email_Verification_Subject, body)) return false;

                // save for ddos prevention
                abuse.EmailSendingSucceeded = true;

                return true;
            }
        }

        public bool SendPasswordResetEmail(SantaUser user)
        {
            const string ddosPreventionKeyPrefix = "santa.user.pasword_reset_for_";
            // prevent abuse:
            var cacheKey = ddosPreventionKeyPrefix + user.Id;
            using (var abuse = new EmailAbuseProtection(cacheKey, _configProvider.PasswordResetCooldown))
            {
                var token = _passwordResetTokenSource.GetAccessCodeFor(
                    user.GetPasswordResetTokenGenerationInputString());

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var link = urlHelper.Action("ResetPassword", "Account", new { userId = user.Id, token },
                    HttpContext.Current.Request.Url.Scheme);

                var body = string.Format(Resources.Global.Email_PasswordReset_Body, user.DisplayName, link, _configProvider.PasswordResetValidFor.Minutes);

                if (!SendEmail(user.Email, Resources.Global.Email_PasswordReset_Subject, body)) return false;

                // save for ddos prevention
                abuse.EmailSendingSucceeded = true;

                return true;
            }
        }

        public bool SendAssignmentEmail(SantaUser user, SantaUser target)
        {
            throw new System.NotImplementedException();
        }

        public bool SendAbandonmentEmail(SantaUser user)
        {
            throw new System.NotImplementedException();
        }

        private bool SendEmail(string to, string subject, string body)
        {
            if (_configProvider.UseMailgun)
            {
                return _mailgun.SendEmail(to, subject, body);
            }

            // else use SMTP
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
                    Log.Error(smptException, $"{nameof(SendEmail)}: Error while sending email");
                    return false;
                }
            }
        }
    }
}