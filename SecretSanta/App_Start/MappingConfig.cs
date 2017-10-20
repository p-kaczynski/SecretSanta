using System;
using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Models;

namespace SecretSanta
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(RegisterMappings);
            Mapper.AssertConfigurationIsValid();
        }

        private static void RegisterMappings(IMapperConfigurationExpression cfg)
        {
            var encryptionProvider = DependencyResolver.Current.GetService<IEncryptionProvider>();
            var countryProvider = DependencyResolver.Current.GetService<CountryProvider>();

            cfg.CreateMap<RegistrationPostModel, SantaUser>()
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.ResolveUsing(post => encryptionProvider.CalculatePasswordHash(post.Password)))
                .ForMember(dest=>dest.Country, opt=>opt.ResolveUsing(post=>countryProvider.ById[post.Country.Id].ThreeLetterIsoCode))
                .ForMember(dest=>dest.EmailConfirmed, opt=>opt.Ignore())
                .ForMember(dest=>dest.AdminConfirmed, opt=>opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest=>dest.CreateDate, opt=>opt.ResolveUsing(_=> DateTime.Now))
                ;

            cfg.CreateMap<SantaUser, SantaSecurityUser>()
                .ForMember(dest => dest.Id, opt => opt.ResolveUsing(model => SantaSecurityUser.GetId(model.Id, false)))
                .ForMember(dest => dest.UserName, opt => opt.ResolveUsing(model => model.Email));

            cfg.CreateMap<SantaAdminPostModel, SantaAdmin>()
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.ResolveUsing(post => encryptionProvider.CalculatePasswordHash(post.Password)))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            cfg.CreateMap<SantaAdmin, SantaAdminPostModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            cfg.CreateMap<ISettingsRepository, SettingsViewModel>();

            cfg.CreateMap<SantaUser, UserHomeViewModel>()
                .ForMember(dest => dest.Assignment, opt => opt.Ignore());

            cfg.CreateMap<SantaUser, AssignmentViewModel>()
                .ForMember(dest=>dest.Country, opt=>opt.ResolveUsing(src=>countryProvider.ByThreeLetterCode[src.Country].Name));

            cfg.CreateMap<PasswordResetViewModel, PasswordResetModel>()
                .ForMember(dest => dest.PasswordBytes,
                    opt => opt.ResolveUsing(post => encryptionProvider.CalculatePasswordHash(post.NewPassword)));

            cfg.CreateMap<SantaUser, SantaUserViewModel>()
                .ForMember(dest => dest.Country,
                    opt => opt.ResolveUsing(src => new CountryEntryViewModel{Id = countryProvider.ByThreeLetterCode[src.Country].Id}));

            cfg.CreateMap<SantaUserPostModel, SantaUser>()
                // let's do explicit for safety
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.FacebookProfileUrl))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.City))
                .ForMember(dest => dest.DisplayName, opt=>opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.Country, opt => opt.ResolveUsing(post => countryProvider.ById[post.Country.Id].ThreeLetterIsoCode))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.SentAbroad))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Note))
                .ForAllOtherMembers(opt=>opt.Ignore());
        }
    }
}