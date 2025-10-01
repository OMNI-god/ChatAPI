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
        private readonly JwtOptions jwtOptions;
        private readonly AppAuthDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<TokenRepository> logger;

        public TokenRepository(JwtOptions jwtOptions, AppAuthDbContext context, IHttpContextAccessor httpContextAccessor,
        ILogger<TokenRepository> logger)
        {
            this.jwtOptions = jwtOptions;
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public (string, DateTime) generateJWTToken(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName,user.UserName??string.Empty),
                new(JwtRegisteredClaimNames.Email,user.Email??string.Empty),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iss,jwtOptions.Issuer),
                new(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            claims.AddRange(jwtOptions.Audience.Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtOptions.AccessTokenExpirationInMinutes));

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience.FirstOrDefault(),
                claims: claims,
                expires: expiry,
                signingCredentials: credentials,
                notBefore: DateTime.UtcNow
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
        }

        public async Task<(RefreshToken, DateTime)> generateRefreshToken(User user, string jwtToken, CancellationToken ct = default)
        {
            var expiry = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtOptions.RefreshTokenExpirationInDays));

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            var hashToken = Sha256(rawToken);

            var rToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Refresh_Token_Hash = hashToken,
                JWT_Token = jwtToken,
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
            .AnyAsync(x => x.UserId == userID && x.Refresh_Token_Hash == hash && !x.IsExpired, ct);
        }
        public async Task invalidateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default)
        {
            var hash = Sha256(rawToken);
            var entity = await context.RefreshTokens.FirstOrDefaultAsync(r => r.Refresh_Token_Hash == hash && r.UserId == userID, ct);
            if (entity != null)
            {
                entity.Expires = DateTime.UtcNow.AddDays(-1);
                await context.SaveChangesAsync(ct);
            }
        }

        public (bool isPresent, string refreshToken) isPresent()
        {
            var refreshToken = httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            return (refreshToken != null, refreshToken);
        }

        public async Task<RefreshToken> getRefreshTokenData(User user)
        {
            string refreshToken = httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return null;
            var refreshTokenDBData = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Refresh_Token_Hash == Sha256(refreshToken) || x.UserId == user.Id);
            return refreshTokenDBData;
        }

        public (ClaimsPrincipal? Principal, DateTime? Expiry, string? RawToken) JwtValidator()
        {
            string token = httpContextAccessor.HttpContext.Request.Cookies["accessToken"];
            if (string.IsNullOrWhiteSpace(token))
                return (null, null, null);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudiences = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                var expiry = jwtToken?.ValidTo;

                return (principal, expiry, token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "JWT validation failed in {MethodName}", nameof(JwtValidator));
                return (null, null, null);
            }
        }

        public async Task<(RefreshToken refreshToken, DateTime expiry)> updateRefreshTokenData(RefreshToken refreshTokenData, string? jwtToken, bool IsAuthenticated, CancellationToken ct = default)
        {

            var expiry = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtOptions.RefreshTokenExpirationInDays));

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            var hashToken = Sha256(rawToken);
            if (IsAuthenticated)
            {
                //update refresh token
                refreshTokenData.Refresh_Token_Hash = hashToken;
                refreshTokenData.RawToken = rawToken;
                refreshTokenData.Expires = expiry;
            }
            else
            {
                //update both refresh token and jwt token
                refreshTokenData.Refresh_Token_Hash = hashToken;
                refreshTokenData.RawToken = rawToken;
                refreshTokenData.Expires = expiry;
                refreshTokenData.JWT_Token = jwtToken;
            }

            context.RefreshTokens.Update(refreshTokenData);
            await context.SaveChangesAsync(cancellationToken: ct);
            return (refreshTokenData, expiry);
        }
    }
}
