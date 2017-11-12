using System;
using System.Web;

namespace SecretSanta.Helpers
{
    public static class FacebookUriHelper
    {
        public static string GetUniformFacebookUri(string fbUriString)
        {
            const string profilePath = "profile.php";
            const string idQueryParameter = "id";
            const string fbAuthority = "www.facebook.com";

            if (string.IsNullOrWhiteSpace(fbUriString) || !Uri.TryCreate(fbUriString,UriKind.Absolute, out var uri))
                return null;
            
            var trimmedPath = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrEmpty(trimmedPath))
                return null;

            if (trimmedPath.Equals(profilePath, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(uri.Query))
                    return null;

                var query = HttpUtility.ParseQueryString(uri.Query);
                var id = query[idQueryParameter];
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                query.Clear();
                query.Add(idQueryParameter,id);

                var uriBuilder = new UriBuilder
                {
                    Scheme = Uri.UriSchemeHttps,
                    Host = fbAuthority,
                    Path = profilePath,
                    Query = query.ToString()
                };

                return uriBuilder.ToString();
            }

            return new UriBuilder{ Scheme = Uri.UriSchemeHttps, Host = fbAuthority, Path = trimmedPath}.ToString();
        }
    }
}