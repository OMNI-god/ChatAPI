using Microsoft.EntityFrameworkCore;
using SignalR_Test.Contexts;
using SignalR_Test.Models;
using SignalR_Test.Token;

namespace SignalR_Test.Services
{
    public class AuthService
    {
        private readonly AppDbContext context;
        private readonly TokenService tokenService;
        readonly RefreshTokenService refreshTokenService;
        public AuthService(AppDbContext context,TokenService tokenService, RefreshTokenService refreshTokenService)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.refreshTokenService = refreshTokenService;

        }
        public (string jwtToken, string refreshToken) Authenticate(string username, string password)
        {
            var user = context.Users.SingleOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var jwtToken = tokenService.CreateToken(user);
            var refreshToken = refreshTokenService.CreateRefreshToken(user.Id.ToString());

            return (jwtToken, refreshToken.Token);
        }
        public string RefreshJwtToken(string refreshTokenValue, string userId)
        {
            var refreshToken=context.RefreshTokens.FirstOrDefault(x=>x.Token==refreshTokenValue && x.UserId==Guid.Parse(userId));
            if (!(refreshToken!=null||!refreshToken.IsActive))
            {
                var user = context.Users.FirstOrDefault(x => x.Id.ToString() == userId);
                var jwtToken=tokenService.CreateToken(user);
                refreshToken.Revoked = DateTime.UtcNow;
                var newRefreshToken = refreshTokenService.CreateRefreshToken(userId);
                context.SaveChanges();
                return jwtToken;
            }
            return "INVALID";
        }
        public void RevokeToken(string tokenValue)
        {
            var token = context.RefreshTokens.FirstOrDefault(rt => rt.Token == tokenValue);

            if (token == null || token.IsExpired || token.Revoked != null)
            {

            }
            else
            {
                token.Revoked = DateTime.UtcNow;
                context.SaveChanges();
            }
        }
    }
}
