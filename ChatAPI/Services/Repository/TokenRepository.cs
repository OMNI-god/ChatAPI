using ChatAPI.Context;
using ChatAPI.Model.Domain;
using ChatAPI.Services.IRepository;
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

        public (string, DateTime) generateJWTToken(User user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("ID",user.Id.ToString())
            }
            .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))
            .ToList();

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["jwt:AccessTokenExpirationInMinutes"]));

            var audiences = configuration.GetSection("jwt:audiences").Get<string[]>();

            var token = new JwtSecurityToken(
                issuer: configuration["jwt:issuer"],
                audience: null,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            token.Payload[JwtRegisteredClaimNames.Aud] = audiences;

            return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
        }

        public async Task<(RefreshToken, DateTime)> generateRefreshToken(User user)
        {
            var expiry = DateTime.UtcNow.AddDays(Convert.ToDouble(configuration["jwt:RefreshTokenExpirationInDays"]));
            var rToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)),
                Created = DateTime.UtcNow,
                Expires = expiry
            };
            await context.RefreshTokens.AddAsync(rToken);
            await context.SaveChangesAsync();
            return (rToken, expiry);
        }
    }
}
