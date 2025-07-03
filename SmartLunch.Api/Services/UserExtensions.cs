using Microsoft.AspNetCore.Identity;
using SmartLunch.Api.Dtos;
using SmartLunch.Database;

namespace SmartLunch.Api.Services
{
    public static class UserExtensions
    {
        public static User UpdateUserCredentials(
            this User user,
            UpdatedUserDto dto,
            UserManager<User> userManager
        )
        {
            ArgumentNullException.ThrowIfNull(user, $"user cannot be null");
            ArgumentNullException.ThrowIfNull(dto, $"dto cannot be null");

            user.Email = dto.Email;
            user.NormalizedEmail = userManager.NormalizeEmail(dto.Email);
            user.UserName = dto.FullName;
            user.NormalizedUserName = userManager.NormalizeName(dto.FullName);
            user.SecurityStamp = dto.SecurityStamp;
            user.ConcurrencyStamp = dto.ConcurrencyStamp;
            user.PhoneNumber = dto.PhoneNumber;
            user.PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(dto.PhoneNumber);

            return user;
        }
    }
}
