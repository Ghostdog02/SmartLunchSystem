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
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value
            );
        }

        public static UserCreationDto ToUserCreationDto(
            this ClaimsDto dto)
        {
            return new UserCreationDto
            (   0, // Id will be set later
                dto.Email,
                DateTime.UtcNow, // LastLoginDate set to current time
                dto.FullName,
                dto.PhoneNumber
            );
        }
    }
}
