using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Models;

namespace SecretSanta
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(RegisterMappings);
        }

        private static void RegisterMappings(IMapperConfigurationExpression cfg)
        {
            var encryptionProvider = DependencyResolver.Current.GetService<IEncryptionProvider>();
            cfg.CreateMap<SantaUserPostModel, SantaUser>()
                .ForMember(model => model.PasswordHash,
                    opt => opt.ResolveUsing(post => encryptionProvider.CalculatePasswordHash(post.Password)));
        }
    }
}