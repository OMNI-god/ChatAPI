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

        public Task<ICollection<MessageResponseDTO>> getMessagesByUserId(Guid userId)
        {
            ICollection<MessageResponseDTO> message=context.Messages.AsNoTracking().Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => new MessageResponseDTO
                {
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    SentAt = m.SentAt
                }). ToList();
            return null;
        }
    }
}
