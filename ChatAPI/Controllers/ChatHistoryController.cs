using ChatAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatHistoryService chatHistoryService;
        public ChatHistoryController(ChatHistoryService chatHistoryService)
        {
            this.chatHistoryService = chatHistoryService;
        }
        [HttpGet]
        public IActionResult GetChatHistory(string userId)
        {
            return Ok(chatHistoryService.GetChatHistory(userId));
        }
        [HttpPost]
        public IActionResult AddChatHistory(string user_1_Id,string user_2_ID,string data)
        {
            return Ok(chatHistoryService.AddChatHistory(user_1_Id,user_2_ID, data));
        }
    }

}
