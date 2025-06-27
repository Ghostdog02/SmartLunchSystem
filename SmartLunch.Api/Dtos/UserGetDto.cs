namespace SmartLunch.Api.Dtos
{
    public record UserGetDto(int Id, 
                             string Email, 
                             DateTime LastLoginDate);
}
