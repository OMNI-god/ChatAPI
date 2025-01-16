using System.Net;
using System.Text.Json.Nodes;

namespace ChatAPI.Model
{
    //HTTP Request return format
    public struct OperationResult
    {
        public HttpStatusCode HTTPCode;
        public string Message;
        public Object? Payload;
    }
    //Chats return format
    public struct ChatResult
    {
        public string Email;
        public JsonArray Chats;
    }
    public struct Chats
    {
        public Guid id;
        public string sender;
        public string profile_pic;
        public Object[] chat;
    }
    public struct Chat
    {
        public string text;
        public DateTime time;
        public bool isSender;
        public bool isSeen;
    }
}
