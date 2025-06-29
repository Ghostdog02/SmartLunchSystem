namespace SmartLunch.Api.Dtos
{
    public record UserCreationDto(int Id,
                             string Email,
                             DateTime LastLoginDate,
                             string FullName,
                             string PhoneNumber);
}