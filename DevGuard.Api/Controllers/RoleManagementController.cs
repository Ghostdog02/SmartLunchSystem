using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DevGuard.Api.Dtos;
using DevGuard.Database;
using DevGuard.Database.Entities;

namespace DevGuard.Api.Controllers
{
    [Route("api/roleManagement")]
    [ApiController]
    public class RoleManagementController(
        DevGuardDbContext context,
        UserManager<User> userManager
    ) : ControllerBase
    {
        private readonly DevGuardDbContext _context = context;
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
