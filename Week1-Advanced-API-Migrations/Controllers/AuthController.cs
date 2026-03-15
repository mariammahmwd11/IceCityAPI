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
                if (!string.IsNullOrEmpty(result.Token))
                {
                    SetAccessTokenInCookie(result.Token, result.TokenExbireAt);
                }
                if (result.IsAuthenticated == false)
                {
                    if (result.Message == "This email is token before")
                    {
                        return Conflict(result.Message);
                    }
                    else if (result.Message == "There is No Owner Match this email")
                    {
                        return BadRequest(result.Message);
                    }
                }
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
                if (!string.IsNullOrEmpty(result.Token))
                {
                    SetAccessTokenInCookie(result.Token, result.TokenExbireAt);
                }
                if (result.IsAuthenticated==false)
                {
                    return BadRequest(result.Message);
                }
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
            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
         
        }
        private void SetAccessTokenInCookie(string key, DateTime ExbireAt)
        {
            var cookieOption = new CookieOptions
            {
                Expires = ExbireAt.ToLocalTime(),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true

            };
            Response.Cookies.Append("AccessTokenInCookie", key, cookieOption);
        }
    }
}