using SignalR_Test.Contexts;
using SignalR_Test.Models;
using System.Security.Cryptography;

namespace SignalR_Test.Token
{
    public class RefreshTokenService
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        public RefreshTokenService(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;

        }
        public RefreshToken CreateRefreshToken(string ID)
        {
            RefreshToken token = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = GenerateToken(),
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["jwt:RefreshTokenExpirationInDays"])), 
                Created = DateTime.UtcNow,
                UserId = Guid.Parse(ID)

            };
            context.RefreshTokens.Add(token);
            context.SaveChanges();  
            return token;
        }

        private string GenerateToken()
        {
            var randomBytes = new byte[32];
            using (var rndm= RandomNumberGenerator.Create())
            {
                rndm.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
