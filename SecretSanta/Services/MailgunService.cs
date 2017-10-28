using System.Net;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using SecretSanta.Common.Interface;

namespace SecretSanta.Services
{
    public class MailgunService
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigProvider _configProvider;
        private const string SendMessageEndpoint = "messages";

        public MailgunService(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public bool SendEmail(string to, string subject, string body)
        {
            var client = new RestClient(_configProvider.MailgunBaseDomain);
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                    _configProvider.MailgunApiKey);

            var request = new RestRequest(SendMessageEndpoint, Method.POST);
            request.AddParameter("from", _configProvider.MailgunFrom);
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("text", body);
            
            var result = client.Execute(request);
            Log.Trace($"Mailgun response ({result.StatusCode}): {result.Content}");
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                default:
                    Log.Error($"Cannot send a message to {to} - got {result.StatusCode} back. Response: {result.Content}");
                    return false;
            }
        }
    }
}