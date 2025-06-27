namespace SmartLunch.Api.Dtos
{
    public record ClaimsDto(string? FirstName, string? Email)
    {
        public void IsAnyClaimTypeNull()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(Email))
            {
                throw new ArgumentNullException("Claim types cannot be null or empty.");
            }
        }
    }
}
