﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public class SetupController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IConfigProvider _configProvider;
        private readonly UserManager<SantaSecurityUser, string> _userManager;
        private readonly ISantaAdminProvider _adminProvider;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IUserRepository _userRepository;

        public SetupController(IConfigProvider configProvider, UserManager<SantaSecurityUser, string> userManager, ISantaAdminProvider adminProvider, IEncryptionProvider encryptionProvider, IUserRepository userRepository, IMapper mapper)
        {
            _configProvider = configProvider;
            _userManager = userManager;
            _adminProvider = adminProvider;
            _encryptionProvider = encryptionProvider;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            if (!_configProvider.DevMode)
                return RedirectToAction("Index", "Home");

            // See if we need to create default admin account
            if (!_adminProvider.GetAllAdmins().Any())
                await CreateDefaultAdminAsync();

            if (_configProvider.GenerateTestData)
                GenerateTestData();


            return View();
        }

        private void GenerateTestData()
        {
            foreach (var postModel in TestUsers)
            {
                var model = _mapper.Map<SantaUser>(postModel);
                _userRepository.InsertUser(model);
            }   
        }

        private Task CreateDefaultAdminAsync()
        {
            var santaAdmin = new SantaAdmin
            {
                UserName = "admin",
                PasswordHash = _encryptionProvider.CalculatePasswordHash("admin")
            };
            return _userManager.CreateAsync(santaAdmin);
        }

        private static readonly RegistrationPostModel[] TestUsers =
        {
            new RegistrationPostModel
            {
                DisplayName = "Janusz Maj",
                Email = "janusz.maj@example.local",
                FacebookProfileUrl = "example.local/janusz.maj",
                FullName = "Janusz Maj",
                AddressLine1 = "1/1",
                AddressLine2 = "Włodarzewska 1",
                City = "Brześć",
                Country = new CountryEntryViewModel{Id = 616},
                Password = "LubięPlacki12345",
                PostalCode = "12-345",
                Note = "Bardzo lubię naleśniki",
                SendAbroad = SendAbroadOption.Can
            },
            new RegistrationPostModel
            {
                DisplayName = "Zenek",
                Email = "z.kowalski@example.local",
                FacebookProfileUrl = "example.local/zkowal",
                FullName = "Zenon Kowalski",
                AddressLine1 = "3/1",
                AddressLine2 = "Włodarzewska 1",
                City = "Warszawa",
                Country = new CountryEntryViewModel{Id = 616},
                Password = "OjejJakieHasło!",
                PostalCode = "55-234",
                Note = "Chciałbym kucyka",
                SendAbroad = SendAbroadOption.Want
            },
            new RegistrationPostModel
            {
                DisplayName = "Ania",
                Email = "anna.radosc@example.local",
                FacebookProfileUrl = "example.local/anka",
                FullName = "Anna Radośc",
                AddressLine1 = "43",
                AddressLine2 = "Opaczewska",
                City = "Kraków",
                Country = new CountryEntryViewModel{Id = 616},
                Password = "siedemLiczbąTo7",
                PostalCode = "11-234",
                Note = "Chciałbym kucyka",
                SendAbroad = SendAbroadOption.WillNot
            },
            new RegistrationPostModel
            {
                DisplayName = "TęgiRuchacz",
                Email = "prezes.nowak@example.local",
                FacebookProfileUrl = "example.local/nowatex",
                FullName = "Krzysztof Nowak",
                AddressLine1 = "2 m. 9",
                AddressLine2 = "Biznesowa 12",
                City = "Poznań",
                Country = new CountryEntryViewModel{Id = 616},
                Password = "kolejorzpany",
                PostalCode = "33-432",
                Note = "Nie wiem co tu wpisać",
                SendAbroad = SendAbroadOption.WillNot
            },
            new RegistrationPostModel
            {
                DisplayName = "xxxtentatcion",
                Email = "mirek@example.local",
                FacebookProfileUrl = "example.local/mirek2342",
                FullName = "Mirosław Żyżyński",
                AddressLine1 = "12 Adress Lane",
                City = "Coventry",
                Country = new CountryEntryViewModel{Id = 826},
                Password = "football1",
                PostalCode = "C1P23",
                Note = "kończą mi się pomysły",
                SendAbroad = SendAbroadOption.Can
            },
            new RegistrationPostModel
            {
                DisplayName = "stasiek",
                Email = "szagran@example.local",
                FacebookProfileUrl = "example.local/stachu",
                FullName = "Stasio Zagraniczny",
                AddressLine1 = "23 Dunbarton Road",
                City = "Glasgow",
                Country = new CountryEntryViewModel{Id = 826},
                Password = "celticMonTheHoops",
                PostalCode = "G11P34",
                Note = "ile można xD",
                SendAbroad = SendAbroadOption.Want
            },
            new RegistrationPostModel
            {
                DisplayName = "iza12",
                Email = "izabellla@example.local",
                FacebookProfileUrl = "example.local/wal_izka",
                FullName = "Izabela Łęcka",
                AddressLine1 = "12 Rou de Chateau",
                City = "Paris",
                Country = new CountryEntryViewModel{Id = 250},
                Password = "ChannelNo5",
                PostalCode = "P123423",
                Note = "Jedna osoba we francji",
                SendAbroad = SendAbroadOption.WillNot
            },
            new RegistrationPostModel
            {
                DisplayName = "panjanek",
                Email = "janek@example.local",
                FacebookProfileUrl = "example.local/jan",
                FullName = "Jan Iksiński",
                AddressLine1 = "12 Viking Street xD",
                City = "Stokholm",
                Country = new CountryEntryViewModel{Id = 752},
                Password = "KotwicaKołobrzeg1995",
                PostalCode = "GBED-234-3FF",
                Note = "Jedna osoba w Szwecji",
                SendAbroad = SendAbroadOption.Want
            }
        };
    }
}