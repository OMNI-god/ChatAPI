using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Model.Domain
{
    public class User:IdentityUser<Guid>
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
    }
}
