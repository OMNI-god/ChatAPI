namespace SignalR_Test.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
