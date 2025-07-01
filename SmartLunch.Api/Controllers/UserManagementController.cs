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
        public async Task<ActionResult<UserDetailsDto>> GetAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            UserDetailsDto dto = user.MapUserToDto();

            return Ok(dto);
        }

        // POST api/userManagement
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] UserCreationDto newUser)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                return Conflict($"User with email {newUser.Email} already exists.");
            }

            User user = newUser.ToEntity();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var readDto = user.MapUserToDto();

            return CreatedAtRoute("GetUserById",
                                  new { id = user.Id },
                                  value: readDto);
        }

        // PUT api/<userManagement/5
        [HttpPut("{id}")]
        public async Task<IResult> Put(int id, [FromBody] UpdatedUserDto dto)
        {
            User? existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return Results.NotFound($"User with id {id} was not found.");
            }

            _context.Entry(existingUser)
                .CurrentValues
                .SetValues(dto.ToEntity(id));

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }

        // DELETE api/<UserManagementController>/5
        [HttpDelete("{id}")]
        public async Task<IResult> Delete(int id)
        {
            User? existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return Results.NotFound($"User with id {id} was not found.");
            }

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
