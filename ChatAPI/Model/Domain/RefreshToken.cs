using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Model.Domain
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; } = default!;
        [NotMapped]
        public string? RawToken { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public bool IsActive => !IsExpired;
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
