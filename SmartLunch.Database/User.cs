﻿using Microsoft.AspNetCore.Identity;

namespace SmartLunch.Database
{
    public class User : IdentityUser<int>
    {
        public DateTime RegistrationDate { get; set; }

        public DateTime LastLoginDate { get; set; }
    }
}
