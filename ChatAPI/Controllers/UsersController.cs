using ChatAPI.Model;
using ChatAPI.Model.DTO;
using ChatAPI.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignalR_Test.ConnectionManager;
using System.Net;
using System.Text;

namespace SignalR_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            //throw new Exception("tesr");
            var loginResponse = await userRepository.login(loginRequestDTO,HttpContext.RequestAborted);
            if (loginResponse != null)
            {
                Response.Cookies.Append("accessToken", loginResponse.token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = loginResponse.tokenExpiry
                });
                Response.Cookies.Append("refreshToken", loginResponse.refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = loginResponse.refreshTokenExpiry
                });

                return Ok(loginResponse);
            }
            return Unauthorized("Invalid username or password");
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterRequestDTO>> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var registerResponse = await userRepository.register(registerRequestDTO);
            if (registerResponse != null)
            {
                return CreatedAtAction(nameof(Register), registerResponse);
            }
            return Problem(statusCode: 400, title: "Registration failed", detail: registerResponse?.response);
        }
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken", new CookieOptions { Secure = true, SameSite = SameSiteMode.None });
            return NoContent();
        }
    }
}
