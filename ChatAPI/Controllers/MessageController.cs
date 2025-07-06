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
        public async Task<IActionResult> GetMessages(Guid userId)
        {
            return Ok(await messageRepository.getMessagesByUserId(userId));
        }
    }
}
