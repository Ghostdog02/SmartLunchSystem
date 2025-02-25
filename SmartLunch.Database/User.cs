using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartLunch.Database
{
    public class User : IdentityUser<int>
    {
        public DateTime RegistrationDate { get; set; }

        public DateTime LastLoginDate { get; set; }
    }
}
