using SignalR_Test.Contexts;
using SignalR_Test.Token;

namespace SignalR_Test.Services
{
    public class AuthService
    {
        private readonly AppDbContext context;
        private readonly TokenService tokenService;
        readonly RefreshTokenService refreshTokenService;
        public AuthService(AppDbContext context, TokenService tokenService, RefreshTokenService refreshTokenService)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.refreshTokenService = refreshTokenService;

        }
        public (string jwtToken, string refreshToken,string userId) Authenticate(string username, string password)
        {
            string jwtToken = string.Empty, refreshToken = string.Empty;
            var user = context.Users.SingleOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = context.RefreshTokens.FirstOrDefault(rt => rt.UserId == user.Id);
            if (token != null)
            {
                if (token.IsExpired)
                {
                    jwtToken = tokenService.CreateToken(user);
                    refreshToken = refreshTokenService.CreateRefreshToken(user.Id.ToString()).Token;
                    context.RefreshTokens.Remove(token);
                    context.SaveChanges();
                }
                else
                {
                    jwtToken = tokenService.CreateToken(user);
                    refreshToken = token.Token;
                }
            }
            else
            {
                jwtToken = tokenService.CreateToken(user);
                refreshToken = refreshTokenService.CreateRefreshToken(user.Id.ToString()).Token;
            }


            return (jwtToken, refreshToken,user.Id.ToString());
        }
        public string RefreshJwtToken(string refreshTokenValue, string userId)
        {
            var refreshToken = context.RefreshTokens.FirstOrDefault(x => x.Token == refreshTokenValue && x.UserId == Guid.Parse(userId));
            if (!(refreshToken != null || !refreshToken.IsActive))
            {
                var user = context.Users.FirstOrDefault(x => x.Id.ToString() == userId);
                var jwtToken = tokenService.CreateToken(user);
                var newRefreshToken = refreshTokenService.CreateRefreshToken(userId);
                context.SaveChanges();
                return jwtToken;
            }
            return "INVALID";
        }
        public void RevokeToken(string tokenValue)
        {
            var refreshToken = context.RefreshTokens.FirstOrDefault(rt => rt.Token == tokenValue);

            if (refreshToken == null || refreshToken.IsExpired)
            {
                context.RefreshTokens.Remove(refreshToken);
                context.SaveChanges();
            }
            else
            {
                //refreshToken.Revoked = DateTime.UtcNow;
                //context.SaveChanges();
            }
        }
    }
}
