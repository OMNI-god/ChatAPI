using ChatAPI.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SignalR_Test.Contexts;

namespace ChatAPI.Services
{
    public class ChatHistoryService
    {
        private readonly AppDbContext context;
        public ChatHistoryService(AppDbContext context)
        {
            this.context = context;
        }

        public Chats GetChatHistory(string userID)
        {
            var chatHistory = context.ChatHistory.FirstOrDefault(x => x.SenderId == Guid.Parse(userID) || x.ReceiverId == Guid.Parse(userID));
            if (chatHistory != null)
            {
                Chats chats = new Chats
                {
                    id = chatHistory.Id,
                    sender = chatHistory.SenderId.ToString() == userID ? chatHistory.ReceiverId.ToString() : chatHistory.SenderId.ToString(),
                    profile_pic = context.Users.FirstOrDefault(x => x.Id == Guid.Parse(chatHistory.SenderId.ToString() == userID ? chatHistory.ReceiverId.ToString() : chatHistory.SenderId.ToString())).ProfilePicture,
                    chat = JsonConvert.DeserializeObject<Chat[]>(chatHistory.ChatLogs)
                };
                return chats;
            }
            return new Chats();
        }

        public async Task AddChatHistory(string user_1_ID, string user_2_ID, string data)
        {
            try
            {
                var chatHistory = context.ChatHistory.FirstOrDefault(x => (x.SenderId == Guid.Parse(user_1_ID) || x.ReceiverId == Guid.Parse(user_1_ID))&& (x.SenderId == Guid.Parse(user_2_ID) || x.ReceiverId == Guid.Parse(user_2_ID)));
                //update if already present
                if (chatHistory != null)
                {
                    var prevChat = DeserializeJSONObject(chatHistory.ChatLogs) as JArray;
                    var newChat = DeserializeJSONObject(data);

                    if (prevChat != null && newChat != null)
                    {
                        prevChat.Merge(newChat.ChatLogs as JArray);
                        chatHistory.ChatLogs = prevChat.ToString();
                    }
                    else if (newChat != null)
                    {
                        chatHistory.ChatLogs = newChat.ChatLogs.ToString();
                    }

                    context.ChatHistory.Update(chatHistory);
                }
                else
                {
                    var Chat = Newtonsoft.Json.JsonConvert.DeserializeObject<Chat>(data);
                    ChatHistory chats = new ChatHistory
                    {
                        Id = Guid.NewGuid(),
                        SenderId = Guid.Parse(user_1_ID),
                        ReceiverId = Guid.Parse(user_2_ID),
                        ChatLogs = Chat.ToString()
                    };
                    context.Add(chats);
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private dynamic DeserializeJSONObject(string data)
        {
            var jdata = JsonConvert.DeserializeObject<dynamic>(data);
            return jdata;
        }
    }
}
