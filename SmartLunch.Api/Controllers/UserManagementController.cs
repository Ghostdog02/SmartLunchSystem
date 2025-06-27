using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLunch.Api.Controllers
{
    [Route("api/userManagement")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly SmartLunchDbContext _context;
        public UserManagementController(SmartLunchDbContext context)
        {
            _context = context;
        }

        // GET: api/userManagement
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAsync()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        // GET api/userManagement/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserManagementController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserManagementController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserManagementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
