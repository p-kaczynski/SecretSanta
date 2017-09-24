using Dapper.Contrib.Extensions;

namespace SecretSanta.Domain.SecurityModels
{
    public class SantaAdmin : SantaSecurityUser
    {
        [Key]
        public long AdminId
        {
            get => GetId(Id, out var _);
            set => Id = GetId(value, true);
        }

        [Computed]
        public override string Id { get; set; }

        [Computed]
        public override bool IsPrivileged { get; } = true;
    }
}