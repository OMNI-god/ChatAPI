using ChatAPI.Context;
using ChatAPI.Model.DTO;
using ChatAPI.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Services.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppAuthDbContext context;

        public MessageRepository(AppAuthDbContext context)
        {
            this.context = context;
        }
        public Task<bool> addMessage()
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<ChatDTO>> getMessagesByUserId(Guid userId)
        {
            var messages = await context.Messages
                 .AsNoTracking()
                 .Include(x => x.Sender)
                 .Include(x => x.Receiver)
                 .Where(x => x.SenderId == userId || x.ReceiverId == userId)
                 .OrderBy(x => x.SentAt)
                 .ToListAsync();

            // Group messages by conversation partner
            var grouped = messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId);

            var result = new List<ChatDTO>();

            foreach (var group in grouped)
            {
                var firstMessage = group.First();
                var otherUser = firstMessage.SenderId == userId ? firstMessage.Receiver : firstMessage.Sender;

                var chatMessages = group.Select(m => new MessageItemDto
                {
                    Text = m.Text,
                    Time = m.SentAt.ToLocalTime().ToString("h:mm tt"),
                    IsSender = m.SenderId == userId
                }).ToList();

                var chatDto = new ChatDTO
                {
                    Id = otherUser.Id,
                    Sender = otherUser.UserName,
                    ProfilePic = "",
                    Chat = chatMessages
                };

                result.Add(chatDto);
            }

            return result;
        }
    }
}
