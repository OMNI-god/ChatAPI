using ChatAPI.Model;
using ChatAPI.Model.DTO;
using ChatAPI.Services.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignalR_Test.ConnectionManager;
using System.Net;
using System.Text;

namespace SignalR_Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            //throw new Exception("tesr");
            var loginResponse = await userRepository.login(loginRequestDTO);
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

                return Ok(new
                {
                    id=loginResponse.Id,
                    userName=loginResponse.userName,
                    email=loginResponse.email
                });
            }
            return Unauthorized("Invalid username or password");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequestDTO)
        {
            var registerResponse = await userRepository.register(registerRequestDTO);
            if (registerResponse != null)
            {
                return Ok(registerResponse);
            }
            return BadRequest();
        }
        
    }
}
