using SecretSanta.Domain.Attributes;

namespace SecretSanta.Domain.Models
{
    public class SantaUser : ModelBase
    {
        public string Email { get; set; }
        public string PerUserSalt { get; set; }
        public string PasswordHash { get; set; }
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

        protected override string IVSource => Email + PerUserSalt;
    }
}