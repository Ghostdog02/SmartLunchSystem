using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartLunch.Database
{
    public class User : IdentityUser<int>
    {
        [Required]
        public override string? UserName { get; set; }

        [Required]
        //[RegularExpression("/([+]359[0-9]{8})|(02[0-9]{7})|(08(0|9|8)([0-9]{7,8}))/gm")]
        //[Phone]
        public override string? PhoneNumber { get; set; }

        [Required]
        //[EmailAddress]
        public override string? Email { get; set; }

        public string? Role { get; set; }

    }
}
