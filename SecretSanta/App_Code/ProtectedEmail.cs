using System.Net.Mail;
using JetBrains.Annotations;

namespace SecretSanta
{
    public class ProtectedEmail
    {
        private readonly bool _parsedCorrectly;
        private readonly MailAddress _mailAddress;

        public ProtectedEmail(string address)
        {
            try
            {
                _mailAddress = new MailAddress(address);
                _parsedCorrectly = true;
            }
            catch
            {
                // ignore
            }
        }

        public override string ToString()
        {
            if (!_parsedCorrectly)
                return string.Empty;

            return $"{Censor(_mailAddress.User)}@{Censor(_mailAddress.Host)}";
        }

        [NotNull]
        private static string Censor(string input)
        {
            if (input == null)
                return string.Empty;

            if (input.Length < 3)
                return input;

            var chars = input.ToCharArray();
            for (var i = 1; i < chars.Length - 1; ++i)
                chars[i] = '*';

            return new string(chars);
        }
    }
}