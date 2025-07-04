using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Api.Dtos;
using SmartLunch.Api.Mapping;
using SmartLunch.Api.Services;
using SmartLunch.Database;
using SmartLunch.Database.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLunch.Api.Controllers
{
    [Route("api/userManagement")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UserManagementController(
        SmartLunchDbContext context,
        UserManager<User> userManager
    ) : ControllerBase
    {
        private readonly SmartLunchDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        // GET: api/userManagement
        [HttpGet]
        public async Task<ActionResult<List<UserDetailsDto>>> GetAsync()
        {
            var users = await _context.Users.MapUsersToDtos().ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        // GET api/userManagement/5
        [HttpGet("{id:int}", Name = "GetUserById")]
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

        // GET api/userManagement/example@gmail.com
        [HttpGet("{email}", Name = "GetUserByEmail")]
        public async Task<ActionResult<UserDetailsDto>> GetAsync(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"User with ID {email} not found.");
            }

            UserDetailsDto dto = user.MapUserToDto();

            return Ok(dto);
        }

        // POST api/userManagement
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] UserCreationDto newUser)
        {
            if (await _userManager.FindByEmailAsync(newUser.Email) != null)
            {
                return Conflict($"User with email {newUser.Email} already exists.");
            }

            User user = newUser.ToEntity();

            // await _context.Users.AddAsync(user);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _context.SaveChangesAsync();

            var readDto = user.MapUserToDto();

            return CreatedAtRoute("GetUserByEmail", new { email = readDto.Email }, value: readDto);
        }

        // POST api/userManagement/assignRole
        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRoleAsync([FromBody] UserRoleDto userRoleDto)
        {
            User? user = await _userManager.FindByIdAsync(userRoleDto.UserId.ToString());

            if (user == null)
            {
                return NotFound($"User with ID {userRoleDto.UserId} not found.");
            }

            var result = await _userManager.AddToRoleAsync(user, userRoleDto.RoleName);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        // PUT api/<userManagement/5
        [HttpPut("{id}")]
        public async Task<IResult> Put(int id, [FromBody] UpdatedUserDto dto)
        {
            // User? existingUser = await _context.Users.FindAsync(id);
            User? existingUser = await _userManager.FindByIdAsync(id.ToString());

            if (existingUser == null)
            {
                return Results.NotFound($"User with id {id} was not found.");
            }

            existingUser.UpdateUserCredentials(dto, _userManager);

            // await _userManager.UpdateAsync(dto.ToEntity(id));
            await _userManager.UpdateAsync(existingUser);
            // _context.Entry(existingUser)
            //     .CurrentValues
            //     .SetValues(dto.ToEntity(id));

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }

        // DELETE api/<UserManagementController>/5
        [HttpDelete("{id}")]
        public async Task<IResult> Delete(int id)
        {
            User? existingUser = await _userManager.FindByIdAsync(id.ToString());

            if (existingUser == null)
            {
                return Results.NotFound($"User with id {id} was not found.");
            }

            await _userManager.DeleteAsync(existingUser);
            // _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
