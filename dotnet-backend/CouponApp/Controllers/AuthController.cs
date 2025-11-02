using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using CouponApp.Services;
using CouponApp.DTOs;

namespace CouponApp.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public AuthController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet("windows-auth")]
        [Authorize]
        public async Task<ActionResult<TokenDto>> WindowsAuth()
        {
            // Get the Windows user identity
            var windowsUser = HttpContext.User?.Identity?.Name;
            
            if (string.IsNullOrEmpty(windowsUser))
            {
                return Unauthorized("Windows authentication failed");
            }

            // Extract just the username part (DOMAIN\username -> username)
            var parts = windowsUser.Split('\\');
            var username = parts.Length > 1 ? parts[1] : windowsUser;

            // Get or create user in our database
            var user = await _userService.GetUserByUsername(username);
            if (user == null)
            {
                // Create user with default role
                var userCreateDto = new UserCreateDto
                {
                    Username = username,
                    Email = $"{username}@company.com",
                    Password = "default_password", // This won't be used for Windows auth
                    Role = "User" // Default role
                };
                
                user = await _userService.CreateUser(userCreateDto);
            }

            // Generate JWT token
            var token = _authService.GenerateToken(user);
            return Ok(new TokenDto { AccessToken = token, TokenType = "bearer" });
        }

        [HttpPost("token")]
        public async Task<ActionResult<TokenDto>> Login([FromForm] LoginDto loginDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username and password are required");
            }

            var user = await _userService.AuthenticateUser(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Incorrect username or password");
            }

            var token = _authService.GenerateToken(user);
            return Ok(new TokenDto { AccessToken = token, TokenType = "bearer" });
        }
    }
}