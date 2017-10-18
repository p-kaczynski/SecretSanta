using System;
using System.Web;
using System.Web.Caching;

namespace SecretSanta.Services
{
    public class EmailAbuseProtection : IDisposable
    {
        public class EmailAbuseException : Exception
        {
            public TimeSpan ExpiryTime { get; }
            public EmailAbuseException(TimeSpan expiryTime)
            {
                ExpiryTime = expiryTime;
            }
        }

        public bool EmailSendingSucceeded { get; set; }

        private readonly string _key;
        private readonly TimeSpan _expiry;

        public EmailAbuseProtection(string key, TimeSpan expiry)
        {
            _key = key;
            _expiry = expiry;
            Check();
        }

        private void Check()
        {
            if(HttpRuntime.Cache.Get(_key) is DateTime setOn && DateTime.Now < setOn + _expiry)
                throw new EmailAbuseException(_expiry - (DateTime.Now - setOn));
        }

        public void Dispose()
        {
            if (EmailSendingSucceeded)
                HttpRuntime.Cache.Add(_key, DateTime.Now, null, DateTime.Now + _expiry, Cache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable, null);
        }
    }
}