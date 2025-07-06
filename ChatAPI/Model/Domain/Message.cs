using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Model.Domain
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
