using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Newtonsoft.Json;
using SecretSanta.Common.Interface;
using SecretSanta.Models;

namespace SecretSanta
{
    public class CountryProvider
    {
        public CountryEntry Default { get; }
        public IReadOnlyDictionary<int, CountryEntry> ById { get; }
        public IReadOnlyDictionary<string, CountryEntry> ByTwoLetterCode { get; }
        public IReadOnlyDictionary<string, CountryEntry> ByThreeLetterCode { get; }
        public CountryProvider(IConfigProvider configProvider)
        {
            var pathToData = "~/App_Data/countries/data";
            const string fileName = "countries.json";

            pathToData = HostingEnvironment.MapPath(pathToData);


            var code = configProvider.UICultureTwoLetterCode;
            if (!Directory.Exists(Path.Combine(pathToData, code)))
                code = "en";

            var list = JsonConvert.DeserializeObject<List<CountryEntry>>(File.ReadAllText(Path.Combine(pathToData, code, fileName)));

            Default = list.SingleOrDefault(c => c.TwoLetterIsoCode == configProvider.UICultureTwoLetterCode) ??
                      list.SingleOrDefault(c => c.TwoLetterIsoCode == "us");
            ById = list.ToDictionary(c => c.Id, c => c);
            ByTwoLetterCode = list.ToDictionary(c => c.TwoLetterIsoCode);
            ByThreeLetterCode = list.ToDictionary(c => c.ThreeLetterIsoCode);
        }
    }
}