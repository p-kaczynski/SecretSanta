using System;
using Microsoft.AspNet.Identity;

namespace SecretSanta.Domain.SecurityModels
{
    public class SantaSecurityUser : IUser
    {
        private const string AdminPrefix = "A";
        private const string StandardUserPrefix = "S";
        private const char Delimiter = ':';

        public static string GetId(long userId, bool admin)
        {
            return $"{(admin ? AdminPrefix : StandardUserPrefix)}{Delimiter}" + userId;
        }

        public static long GetId(string userId, out bool isAdmin)
        {
            var split = userId.Split(Delimiter);
            if (split.Length != 2)
                throw new ArgumentException($"Incorrect format of {nameof(userId)}:{userId}", nameof(userId));

            switch (split[0]) // case as we might add another case later, and nicer than if/else if/else
            {
                case StandardUserPrefix:
                    isAdmin = false;
                    break;
                case AdminPrefix:
                    isAdmin = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userId), $"Prefix out of range: {split[0]}");
            }
            if (!long.TryParse(split[1], out var id))
                throw new ArgumentException($"Provided second segment of {nameof(userId)}={split[0]} cannot be parsed into long");

            return id;
        }

        public virtual string Id { get; set; }
        public string UserName { get; set; }
        public virtual bool IsPrivileged { get; } = false;
        public byte[] PasswordHash { get; set; }
    }
}