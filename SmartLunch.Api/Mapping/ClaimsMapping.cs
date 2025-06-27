using SmartLunch.Api.Dtos;
using System.Security.Claims;

namespace SmartLunch.Api.Mapping
{
    public static class ClaimsMapping
    {
        public static ClaimsDto ToClaimsDto(this IEnumerable<Claim> claims)
        {
            return new ClaimsDto(
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            );
        }
    }
}
