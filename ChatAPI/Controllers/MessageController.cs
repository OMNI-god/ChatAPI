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

            return Ok(await messageRepository.getMessagesByUserId(userId));
        }
    }
}
