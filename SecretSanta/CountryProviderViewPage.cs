using System.Web.Mvc;
using SecretSanta.Models;

namespace SecretSanta
{
    public abstract class CountryProviderViewPage : WebViewPage<CountryEntryViewModel>
    {
        public CountryProvider CountryProvider { get; set; }
    }
}