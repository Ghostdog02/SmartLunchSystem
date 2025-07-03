using System.Collections.Generic;
using SmartLunch.Api.Dtos;
using SmartLunch.Database;

namespace SmartLunch.Api.Mapping
{
    public static class UserMapping
    {
        public static UserDetailsDto MapUserToDto(this User user)
        {
            UserDetailsDto dto = new(
                user.Id,
                user.Email!,
                user.LastLoginDate,
                user.UserName!,
                user.RegistrationDate
            );

            return dto;
        }

        public static IQueryable<UserDetailsDto> MapUsersToDtos(this IQueryable<User> users)
        {
            return users.Select(user => user.MapUserToDto());
        }

        public static User ToEntity(this UserCreationDto dto)
        {
            return new User
            {
                Email = dto.Email,
                UserName = dto.FullName,
                SecurityStamp = dto.SecurityStamp,
                ConcurrencyStamp = dto.ConcurrencyStamp,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberConfirmed = dto.PhoneNumber != null || dto.PhoneNumber != string.Empty,
                RegistrationDate = dto.RegistrationDate,
            };
        }

        public static User ToEntity(this UpdatedUserDto dto, int id)
        {
            return new User
            {
                Id = id,
                Email = dto.Email,
                UserName = dto.FullName,
                SecurityStamp = dto.SecurityStamp,
                ConcurrencyStamp = dto.ConcurrencyStamp,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberConfirmed = dto.PhoneNumber != null || dto.PhoneNumber != string.Empty,
            };
        }
    }
}
