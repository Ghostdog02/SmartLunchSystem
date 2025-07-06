using System.Security.Claims;
using SmartLunch.Api.Dtos;

namespace SmartLunch.Api.Mapping
{
    public static class ClaimsMapping
    {
        public static ClaimsDto ToClaimsDto(this IEnumerable<Claim> claims)
        {
            return new ClaimsDto(
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)!.Value
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
                dto.PhoneNumber,
                DateTime.Now
            );

            return userCreationDto;
        }
    }
}
