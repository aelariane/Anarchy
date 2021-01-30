using System;

namespace Antis.Protections
{
    public sealed class URLChecker : IProtection<string>
    {
        bool IProtection.Check(object data)
        {
            string value = data as string;
            if (value != null)
            {
                return Check(value);
            }
            return false;
        }

        public bool Check(string url)
        {
            Uri uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri) && (uri != null && uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}