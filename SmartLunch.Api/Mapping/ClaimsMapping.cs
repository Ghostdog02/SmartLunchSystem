using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SmartLunch.Api.Dtos;

namespace SmartLunch.Api.Mapping
{
    public static class ClaimsMapping
    {
        public static ClaimsDto ToClaimsDto(this IEnumerable<Claim> claims)
        {
            if (!claims.Any())
            {
                throw new ArgumentException($"Given claims are either null or empty");
            }

            return new ClaimsDto(
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            );
        }

        public static UserCreationDto ToUserCreationDto(this ClaimsDto dto)
        {
            var userCreationDto = new UserCreationDto(
                0, // Id will be set later
                dto.Email!,
                dto.FullName!,
                Guid.NewGuid().ToString(), // SecurityStamp
                Guid.NewGuid().ToString(), // ConcurrencyStamp
                string.Empty,
                DateTime.Now
            );

            return userCreationDto;
        }
    }
}
