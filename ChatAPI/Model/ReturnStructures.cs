using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Nodes;

namespace ChatAPI.Model
{
    //HTTP Request return format
    public struct OperationResult
    {
        public HttpStatusCode HTTPCode { get; set; }
        public string Message { get; set; }
        public Object? Payload { get; set; }
    }
    //Chats return format
    public struct ChatResult
    {
        public string Email { get; set; }
        public Object[] Chats { get; set; }
    }
    public struct Chats
    {
        public Guid id { get; set; }
        public string sender { get; set; }
        public string profile_pic { get; set; }
        public Chat[] chat { get; set; }
    }
    public struct Chat
    {
        public string text { get; set; }
        public DateTime time { get; set; }
        public string from { get; set; }
        public bool isSeen { get; set; }
    }
}
