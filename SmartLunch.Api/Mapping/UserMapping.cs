using SmartLunch.Api.Dtos;
using SmartLunch.Database;
using System.Collections.Generic;

namespace SmartLunch.Api.Mapping
{
    public static class UserMapping
    {
        public static UserGetDto MapUserToDto(this User user)
        {
            UserGetDto dto = new UserGetDto(user.Id,
                                            user.Email!,
                                            user.LastLoginDate);

            return dto;

        }

        public static IQueryable<UserGetDto> MapUsersToDtos(
           this IQueryable<User> users)
        {
            return users.Select(user => user.MapUserToDto());
        }
    }
}
