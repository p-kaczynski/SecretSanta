using System;
using SecretSanta.Domain.Attributes;

namespace SecretSanta.Domain.Models
{
    public class SantaUser : ModelBase
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
        [DataProtection]
        public string Country { get; set; }
        [DataProtection]
        public string Note { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool AdminConfirmed { get; set; }
        public DateTime CreateDate { get; set; }

        protected override string IVSource => Email + DisplayName;
    }
}