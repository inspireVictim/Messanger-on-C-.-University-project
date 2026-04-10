using messanger.DTOs;
using messanger.Services;
using Microsoft.AspNetCore.Mvc;

namespace messanger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (result == "User already exists") return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);

            if (result == "User not found" || result == "InvalidPassword") return Unauthorized();

            return Ok(result);
        }
    }
}
