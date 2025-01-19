using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Model
{
    public class ChatHistory
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set;}
        public string ChatLogs { get; set; }
    }
}
