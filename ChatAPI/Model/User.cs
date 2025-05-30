using Microsoft.AspNetCore.Identity;

namespace SignalR_Test.Models
{
    public class User:IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
