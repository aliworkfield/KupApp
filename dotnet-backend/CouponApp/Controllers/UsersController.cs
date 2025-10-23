using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CouponApp.Services;
using CouponApp.Models;
using CouponApp.DTOs;

namespace CouponApp.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString().ToLower(),
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(UserCreateDto userDto)
        {
            // Check if user already exists
            var existingUser = await _userService.GetUserByUsername(userDto.Username);
            if (existingUser != null)
            {
                return BadRequest("User with this username already exists");
            }

            var user = await _userService.CreateUser(userDto);

            var userDtoResult = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString().ToLower(),
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDtoResult);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int skip = 0, [FromQuery] int limit = 100)
        {
            var users = await _userService.GetUsers(skip, limit);
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString().ToLower(),
                CreatedAt = u.CreatedAt
            });

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString().ToLower(),
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UserUpdateDto userDto)
        {
            var user = await _userService.UpdateUser(id, userDto);
            if (user == null)
            {
                return NotFound();
            }

            var userDtoResult = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString().ToLower(),
                CreatedAt = user.CreatedAt
            };

            return Ok(userDtoResult);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> DeleteUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var success = await _userService.DeleteUser(id);
            if (!success)
            {
                return StatusCode(500, "Error deleting user");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString().ToLower(),
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }
    }
}