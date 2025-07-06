using ChatAPI.Model.DTO;

namespace ChatAPI.Services.IRepository
{
    public interface IMessageRepository
    {
        Task<bool> addMessage();
        Task<ICollection<ChatDTO>> getMessagesByUserId(Guid userId);
    }
}
