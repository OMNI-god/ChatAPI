namespace ChatAPI.Model.Domain
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= this.Expires;
        public DateTime Created { get; set; }
        public bool IsActive => !this.IsExpired;
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
