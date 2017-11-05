using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Autofac.Features.Indexed;
using NLog;
using SaturnV;
using SecretSanta.Common;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.Models.Extensions;
using SecretSanta.Helpers;

namespace SecretSanta.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfigProvider _configProvider;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly SmtpClient _smtp;
        private readonly SecureAccessTokenSource _emailConfirmationTokenSource;
        private readonly SecureAccessTokenSource _passwordResetTokenSource;
        private readonly CountryProvider _countryProvider;
        private readonly MailgunService _mailgun;

        public EmailService(IConfigProvider configProvider, IIndex<TokenSourceType,SecureAccessTokenSource> satIndex, CountryProvider countryProvider)
        {
            _configProvider = configProvider;
            _countryProvider = countryProvider;

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

                var body = string.Format(Resources.Global.Email_PasswordReset_Body, user.DisplayName, link, _configProvider.PasswordResetValidFor.TotalMinutes);

                if (!SendEmail(user.Email, Resources.Global.Email_PasswordReset_Subject, body)) return false;

                // save for ddos prevention
                abuse.EmailSendingSucceeded = true;

                return true;
            }
        }

        public bool SendAssignmentEmail(SantaUser user, SantaUser target)
        {
            var body = string.Format(Resources.Global.Email_Assignment_Body, target.FullName, target.AddressLine1, target.AddressLine2, target.PostalCode, target.City, _countryProvider.ByThreeLetterCode[target.Country].Name, target.Note, target.FacebookProfileUrl);

            return SendEmail(user.Email, Resources.Global.Email_Assignment_Subject, body);
        }

        public bool SendAbandonmentEmail(SantaUser user, AbandonmentReason reason)
        {
            var body = string.Format(Resources.Global.Email_Abandonment_Body, user.DisplayName, reason.GetUserFriendlyDescription());

            return SendEmail(user.Email, Resources.Global.Email_Abandonment_Subject, body);
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