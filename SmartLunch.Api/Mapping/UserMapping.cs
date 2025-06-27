using SmartLunch.Api.Dtos;
using SmartLunch.Database;

namespace SmartLunch.Api.Mapping
{
    public class UserMapping
    {
        public UserGetDto MapUserToDto(User user)
        {
            UserGetDto dto = new UserGetDto(user.Id, 
                                            user.Email!, 
                                            user.LastLoginDate);

            return dto;

        }
    }
}
