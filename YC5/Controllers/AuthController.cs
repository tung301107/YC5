using Microsoft.AspNetCore.Mvc;
using YC5.DTOs;
using YC5.Interfaces;

namespace YC5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.Register(dto);
            if (result == "Đăng ký thành công") return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto);
            if (token == null) return Unauthorized("Sai tài khoản hoặc mật khẩu");
            return Ok(new { Token = token });
        }
    }
}