using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Database;

namespace UserManagementApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult<User>> GetAsync()
        {
            
        }
    }
}
