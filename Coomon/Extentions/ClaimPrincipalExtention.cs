using Coomon.Helpers;
using System.Security.Claims;

namespace Coomon.Extentions
{
    public static class ClaimPrincipalExtention
    {
        public static T? GetClaimValue<T>(this ClaimsPrincipal user, string claim)
        {
            var value = user.Claims.FirstOrDefault(x => x.Type == claim)?.Value;
            return value == null ? default : Converter.ConvertFromString<T>(value);
        }
    }
}
