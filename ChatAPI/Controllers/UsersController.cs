using ChatAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignalR_Test.Models;
using SignalR_Test.Services;
using System.Text;

namespace SignalR_Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthService authService;
        private readonly UserService userService;

        public UsersController(AuthService authService,UserService userService)
        {
            this.authService = authService;
            this.userService = userService;
        }

        [HttpPost]
        public IActionResult Login()
        {
            try
            {
                var jdata = RequestBody();
                var (token, refreshToken) = authService.Authenticate((string)jdata.username, (string)jdata.password);
                return Ok(new
                {
                    Token = token,
                    RefreshToken = refreshToken,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                var jdata = RequestBody();
                authService.RevokeToken((string)jdata.refreshToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Register()
        {
            try
            {
                var jdata = RequestBody();
                userService.CreateUserAsync(jdata);
                return Ok(new
                {
                    message="Success",
                    jdata
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private dynamic RequestBody()
        {
            var loginContext = HttpContext.Request.Body;
            using (var sreader = new StreamReader(loginContext, Encoding.UTF8, true))
            {
                var body = sreader.ReadToEndAsync();
                var jdata = JsonConvert.DeserializeObject<dynamic>(body.Result);
                return jdata;
            }
        }
    }
}
