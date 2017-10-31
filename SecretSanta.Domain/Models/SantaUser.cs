using System;
using SecretSanta.Domain.Attributes;
using SecretSanta.Domain.Enums;

namespace SecretSanta.Domain.Models
{
    public class SantaUser : ModelBase, IEquatable<SantaUser>
    {
        public string Email { get; set; }
        public string FacebookProfileUrl { get; set; }
        public byte[] PasswordHash { get; set; }
        public string DisplayName { get; set; }
        [DataProtection]
        public string FullName{ get; set; }
        [DataProtection]
        public string AddressLine1 { get; set; }
        [DataProtection]
        public string AddressLine2 { get; set; }
        [DataProtection]
        public string PostalCode { get; set; }
        [DataProtection]
        public string City { get; set; }
        public string Country { get; set; }
        [DataProtection]
        public string Note { get; set; }
        public bool SentAbroad { get; set; }
        public SendAbroadOption SendAbroad { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool AdminConfirmed { get; set; }
        public DateTime CreateDate { get; set; }

        protected override string IVSource => Email.ToLowerInvariant();
        public bool Equals(SantaUser other)
        {
            return Id.Equals(other?.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SantaUser) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode - what am  I gonna do. It's id...
            return Id.GetHashCode();
        }
    }
}