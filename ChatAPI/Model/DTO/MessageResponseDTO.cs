using ChatAPI.Model.Domain;

namespace ChatAPI.Model.DTO
{
    public class ChatDTO
    {
        public Guid Id { get; set; }
        public string Sender { get; set; }
        public string ProfilePic { get; set; }
        public List<MessageItemDto> Chat { get; set; }
    }
    public class MessageItemDto
    {
        public string Text { get; set; }
        public string Time { get; set; }
        public bool IsSender { get; set; }
    }
}
