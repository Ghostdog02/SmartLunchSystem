namespace SmartLunch.Api.Dtos
{
    public record UpdatedUserDto(
        string Email,
        string FullName,
        string SecurityStamp,
        string ConcurrencyStamp,
        string PhoneNumber,
        DateTime LastLoginDate
    );
}
