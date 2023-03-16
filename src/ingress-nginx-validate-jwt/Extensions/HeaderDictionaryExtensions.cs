using System.Globalization;
using ingress_nginx_validate_jwt.Constants;

namespace ingress_nginx_validate_jwt.Extensions;

public static class HeaderDictionaryExtensions
{
    public static string? GetOriginalUrlValue(this IHeaderDictionary headers)
    {
        return headers.FirstOrDefault(h => h.Key.Equals(CustomHeaders.OriginalUrl, StringComparison.InvariantCultureIgnoreCase)).Value;
    }
}