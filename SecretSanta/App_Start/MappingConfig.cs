using System;
using System.Web.Mvc;
using Autofac;
using AutoMapper;
using JetBrains.Annotations;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Models;

namespace SecretSanta
{
    public static class MappingConfig
    {
        //public static void Configure()
        //{
        //    Mapper.Initialize(RegisterMappings);
        //    Mapper.AssertConfigurationIsValid();
        //}

        [NotNull]
        public static MapperConfiguration GetMapperConfiguration(IComponentContext context)
        {
            var config = new MapperConfiguration(expression => RegisterMappings(expression, context));
            config.AssertConfigurationIsValid();

            return config;
        }

        private static void RegisterMappings([NotNull] IMapperConfigurationExpression cfg, IComponentContext context)
        {
            var encryptionProvider = context.Resolve<IEncryptionProvider>();
            var countryProvider = context.Resolve<CountryProvider>();
            var configProvider = context.Resolve<IConfigProvider>();

            cfg.CreateMap<RegistrationPostModel, SantaUser>()
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom(post => encryptionProvider.CalculatePasswordHash(post.Password, null)))
                .ForMember(dest=>dest.Country, opt=>opt.MapFrom(post=>countryProvider.ById[post.Country.Id].ThreeLetterIsoCode))
                .ForMember(dest=>dest.IsAdult, opt=>opt.MapFrom(post=> post.DateOfBirth.AddYears(configProvider.AdultAge) <= DateTime.Today))
                .ForMember(dest=>dest.EmailConfirmed, opt=>opt.Ignore())
                .ForMember(dest=>dest.AdminConfirmed, opt=>opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest=>dest.CreateDate, opt=>opt.MapFrom(_=> DateTime.Now))
                ;

            cfg.CreateMap<SantaUser, SantaSecurityUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(model => SantaSecurityUser.GetId(model.Id, false)))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(model => model.Email));

            cfg.CreateMap<SantaAdminPostModel, SantaAdmin>()
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom(post => encryptionProvider.CalculatePasswordHash(post.Password,null)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayName, opt => opt.Ignore());

            cfg.CreateMap<SantaAdmin, SantaAdminPostModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            cfg.CreateMap<ISettingsRepository, SettingsViewModel>();

            cfg.CreateMap<SantaUser, UserHomeViewModel>()
                .ForMember(dest => dest.Assignment, opt => opt.Ignore())
                .ForMember(dest => dest.InboundGiftArrived, opt => opt.Ignore())
                .ForMember(dest => dest.InboundGiftEnRoute, opt => opt.Ignore())
                .ForMember(dest => dest.OutboundGiftEnRoute, opt => opt.Ignore())
                .ForMember(dest => dest.OutboundGiftArrived, opt => opt.Ignore())
                .ForMember(dest => dest.AssignmentPerformed, opt => opt.Ignore())
                .ForMember(dest => dest.InboundGiftMissing, opt => opt.Ignore())
                .ForMember(dest => dest.OutboundGiftMissing, opt => opt.Ignore());

            cfg.CreateMap<SantaUser, AssignmentViewModel>()
                .ForMember(dest=>dest.Country, opt=>opt.MapFrom(src=>countryProvider.ByThreeLetterCode[src.Country].Name));

            cfg.CreateMap<PasswordResetViewModel, PasswordResetModel>()
                .ForMember(dest => dest.PasswordBytes,
                    opt => opt.MapFrom(post => encryptionProvider.CalculatePasswordHash(post.NewPassword, null)));

            cfg.CreateMap<SantaUser, SantaUserViewModel>()
                .ForMember(dest => dest.Country,
                    opt => opt.MapFrom(src => new CountryEntryViewModel{Id = countryProvider.ByThreeLetterCode[src.Country].Id}));

            cfg.CreateMap<SantaUserPostModel, SantaUser>()
                // let's do explicit for safety
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt=>opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FacebookProfileUrl, opt=>opt.MapFrom(src => src.FacebookProfileUrl))
                .ForMember(dest => dest.FullName, opt=>opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.AddressLine1, opt=>opt.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.AddressLine2, opt=>opt.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.City, opt=>opt.MapFrom(src => src.City))
                .ForMember(dest => dest.PostalCode, opt=>opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(post => countryProvider.ById[post.Country.Id].ThreeLetterIsoCode))
                .ForMember(dest => dest.SendAbroad, opt => opt.MapFrom(src => src.SendAbroad))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForAllOtherMembers(opt=>opt.Ignore());
        }
    }
}