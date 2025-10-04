using System.Threading.Tasks;
using ChatAPI.Model.DTO;
using ChatAPI.Services.IRepository;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SignalR_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IAntiforgery antiforgery;

        public UsersController(IUserRepository userRepository, IAntiforgery antiforgery)
        {
            this.userRepository = userRepository;
            this.antiforgery = antiforgery;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await userRepository.login(loginRequestDTO, HttpContext.RequestAborted);
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

                AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(HttpContext);
                loginResponse.CsrfToken = tokens.RequestToken;

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
        public async Task<IActionResult> Logout([FromServices] IAntiforgery antiforgery)
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
            Response.Cookies.Delete("accessToken", new CookieOptions { Secure = true, SameSite = SameSiteMode.None });
            return NoContent();
        }
        [HttpGet("searchUser")]
        [Authorize]
        public async Task<IActionResult> SearchUser([FromBody] SearchUserRequestDTO searchUserRequest)
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
            var data = await userRepository.searchUser(searchUserRequest);
            return Ok(data);
        }
    }
}
