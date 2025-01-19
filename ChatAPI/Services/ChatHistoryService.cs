using ChatAPI.Model;
using Microsoft.EntityFrameworkCore;
using SignalR_Test.Contexts;
using System.Security.Cryptography.X509Certificates;

namespace ChatAPI.Services
{
    public class ChatHistoryService
    {
        private readonly AppDbContext context;
        public ChatHistoryService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ChatHistory> GetChatHistory(string senderId, string receiverId)
        {
            var chatHistory = await context.ChatHistory.FirstOrDefaultAsync(x => x.SenderId == Guid.Parse(senderId) && x.ReceiverId == Guid.Parse(receiverId));
            if (chatHistory != null)
            {
                Chats chats = new Chats
                {
                    id = chatHistory.Id,
                    sender = chatHistory.SenderId.ToString(),
                    profile_pic = context.Users.FirstOrDefault(x=>x.Id==Guid.Parse(senderId)).ProfilePicture,
                };
            }
            return chatHistory ?? new ChatHistory();
        }
    }
}
