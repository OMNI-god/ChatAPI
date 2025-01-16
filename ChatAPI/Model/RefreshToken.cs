namespace SignalR_Test.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; }
        public bool IsActive => Created<DateTime.UtcNow;
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
