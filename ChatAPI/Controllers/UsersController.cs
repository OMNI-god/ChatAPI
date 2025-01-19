using ChatAPI.Model;
using ChatAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignalR_Test.ConnectionManager;
using SignalR_Test.Models;
using SignalR_Test.Services;
using System.Net;
using System.Text;

namespace SignalR_Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthService authService;
        private readonly UserService userService;
        private readonly IConnectionManager connectionManager;

        public UsersController(AuthService authService,UserService userService,IConnectionManager connectionManager)
        {
            this.authService = authService;
            this.userService = userService;
            this.connectionManager = connectionManager;
        }

        [HttpPost]
        public IActionResult Login()
        {
            try
            {
                var jdata = RequestBody();
                var (token, refreshToken) = authService.Authenticate((string)jdata.username, (string)jdata.password);
                return Ok(new OperationResult
                {
                    HTTPCode = HttpStatusCode.OK,
                    Message = "Login Success",
                    Payload = new { token = token, refreshToken = refreshToken }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationResult
                {
                    HTTPCode = HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                var jdata = RequestBody();
                authService.RevokeToken((string)jdata.refreshToken);
                connectionManager.RemoveConnection((string)jdata.connectionId); 
                return Ok(new OperationResult
                {
                    HTTPCode = HttpStatusCode.OK,
                    Message = "Logout Success"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationResult
                {
                    HTTPCode = HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        public IActionResult Register()
        {
            try
            {
                var jdata = RequestBody();
                var data=userService.CreateUserAsync(jdata);
                if(data.Result.HTTPCode==HttpStatusCode.OK)
                {
                    return Ok(data.Result);
                }
                else
                {
                    return Conflict(data.Result);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new OperationResult
                {
                    HTTPCode = HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
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
