using Newtonsoft.Json;

namespace SecretSanta.Models
{
    public class CountryEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("alpha2")]
        public string TwoLetterIsoCode { get; set; }
        [JsonProperty("alpha3")]
        public string ThreeLetterIsoCode { get; set; }
    }
}