using Business_Layer.DTO;
using Business_Layer.IServices;
using Data_Access.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace Week1_Advanced_API_Migrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var result = authService.Register(registerDTO);
                return Ok(result); 
            }
            return BadRequest();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO loginDto)
        {
            if (ModelState.IsValid)
            {
                var result = authService.login(loginDto);
                return Ok(result); 
            }
            return BadRequest();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefershTokenDTO refershTokenDTO)
        {
            
            if (string.IsNullOrEmpty(refershTokenDTO.RefereshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var result = await authService.RefreshToken(refershTokenDTO.RefereshToken);
            return Ok(result); 
        }
    }
}