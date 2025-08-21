using System.Net;
using ChatAPI.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }
        [HttpGet]
        [ResponseCache(Duration = 2, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMessages([FromQuery] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            if (pageSize <= 0 || page <= 0 || pageSize > 200) return Problem(statusCode: (int)HttpStatusCode.BadRequest, title: "Invalid paging arguments.");
            var result = await messageRepository.getMessagesByUserId(userId, page, pageSize);
            return Ok(result);
        }
    }
}
