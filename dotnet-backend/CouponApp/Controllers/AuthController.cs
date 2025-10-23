using Microsoft.AspNetCore.Mvc;
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