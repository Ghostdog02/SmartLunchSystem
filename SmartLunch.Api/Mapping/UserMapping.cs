using SmartLunch.Api.Dtos;
using SmartLunch.Database;
using System.Collections.Generic;

namespace SmartLunch.Api.Mapping
{
    public static class UserMapping
    {
        public static UserDetailsDto MapUserToDto(this User user)
        {
            UserDetailsDto dto = new (user.Id,
                                            user.Email!,
                                            user.LastLoginDate,
                                            user.UserName!);

            return dto;

        }

        public static IQueryable<UserDetailsDto> MapUsersToDtos(
           this IQueryable<User> users)
        {
            return users.Select(user => user.MapUserToDto());
        }

        public static User ToEnity(this UserCreationDto dto)
        {
            return new User
            {
                Email = dto.Email,
                LastLoginDate = dto.LastLoginDate,
                UserName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };
        }
    }
}
