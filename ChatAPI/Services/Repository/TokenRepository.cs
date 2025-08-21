using ChatAPI.Context;
using ChatAPI.Model.Domain;
using ChatAPI.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatAPI.Services.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        private readonly AppAuthDbContext context;

        public TokenRepository(IConfiguration configuration, AppAuthDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public (string, DateTime) generateJWTToken(User user, IEnumerable<string> roles)
        {
            var jwtSection = configuration.GetSection("JWT");
            var issuer = jwtSection["Issure"];
            var audience = jwtSection["Audience"];
            var key = jwtSection["Key"];
            var accessTokenExpirationinMinutes = jwtSection["AccessTokenExpirationinMinutes"];

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName,user.UserName),
                new(JwtRegisteredClaimNames.Email,user.Email)
            };
            // .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))
            // .ToList();

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(accessTokenExpirationinMinutes));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials,
                notBefore: DateTime.UtcNow
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
        }

        public async Task<(RefreshToken, DateTime)> generateRefreshToken(User user, CancellationToken ct = default)
        {
            var jwtSection = configuration.GetSection("JWT");
            var refreshTokenExpirationInDays = jwtSection["RefreshTokenExpirationInDays"];

            var expiry = DateTime.UtcNow.AddDays(Convert.ToDouble(refreshTokenExpirationInDays));

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            var hashToken = Sha256(rawToken);

            var rToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hashToken,
                Created = DateTime.UtcNow,
                Expires = expiry,
                RawToken = rawToken
            };
            await context.RefreshTokens.AddAsync(rToken, ct);
            await context.SaveChangesAsync(ct);
            return (rToken, expiry);
        }

        private static string Sha256(string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToHexString(bytes);
            }
        }
        public async Task<bool> validateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default)
        {
            var hash = Sha256(rawToken);
            return await context.RefreshTokens.AsNoTracking()
            .AnyAsync(x => x.UserId == userID && x.TokenHash == hash && !x.IsExpired, ct);
        }
        public async Task invalidateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default)
        {
            var hash = Sha256(rawToken);
            var entity = await context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash && r.UserId == userID, ct);
            if (entity != null)
            {
                entity.Expires = DateTime.UtcNow.AddDays(-1);
                await context.SaveChangesAsync(ct);
            }
        }
    }
}
