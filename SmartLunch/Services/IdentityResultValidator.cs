using Microsoft.AspNetCore.Identity;

namespace SmartLunch.Services
{
    public class IdentityResultValidator
    {
        public void CheckSuccess(IdentityResult result, string description)
        {
            if (!result.Succeeded)
            {
                string errorMessages = string.Join(", ", result.Errors.
                    Select(e => e.Description));

                throw new Exception($"{description}: {string.Join(", ", result.Errors)}");
            }
        }
    }
}
