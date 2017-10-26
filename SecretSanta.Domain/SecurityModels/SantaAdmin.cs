using Dapper.Contrib.Extensions;
using JetBrains.Annotations;

namespace SecretSanta.Domain.SecurityModels
{
    public class SantaAdmin : SantaSecurityUser
    {
        [Key]
        public long AdminId
        {
            get => GetId(Id, out var _);
            [UsedImplicitly]set => Id = GetId(value, true);
        }

        [Computed]
        public override string Id { get; set; }

        [Computed]
        public override bool IsPrivileged { get; } = true;

        [Computed]
        public override string DisplayName
        {
            get => UserName;
            set
            {
            }
        }
    }
}