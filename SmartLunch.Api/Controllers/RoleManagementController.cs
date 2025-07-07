using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Api.Dtos;
using SmartLunch.Database;
using SmartLunch.Database.Entities;

namespace SmartLunch.Api.Controllers
{
    [Route("api/roleManagement")]
    [ApiController]
    public class RoleManagementController(
        SmartLunchDbContext context,
        UserManager<User> userManager
    ) : ControllerBase
    {
        private readonly SmartLunchDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        // GET: api/roleManagement/example@example.com
        [HttpGet("{email}")]
        public async Task<ActionResult<string>> GetRoleByEmail([FromRoute]
            string email)
        {
            User user = await _userManager.FindByEmailAsync(email) ??
                throw new ArgumentException(
                    $"The user with the given email {email} has not been found");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.First();

            return role;
        }
    }
}
