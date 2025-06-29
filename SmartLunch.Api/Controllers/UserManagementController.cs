using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Api.Dtos;
using SmartLunch.Api.Mapping;
using SmartLunch.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLunch.Api.Controllers
{
    [Route("api/userManagement")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
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
            var users = await _context.Users.MapUsersToDtos().ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        // GET api/userManagement/5
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<User>> GetAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var dto = user.MapUserToDto();

            return Ok(dto);
        }

        // POST api/userManagement
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] UserCreationDto newUser)
        {
            User user = newUser.ToEnity();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var readDto = user.MapUserToDto();

            return CreatedAtAction("GetUserById",
                                  new { id = user.Id },
                                  value: readDto);
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
