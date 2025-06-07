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

        public TokenRepository(IConfiguration configuration,AppAuthDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }
        public string generateJWTToken(User user, List<string> roles)
        {
            //throw new Exception("terst");
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
            }.Concat(roles.AsEnumerable().Select(x => new Claim(ClaimTypes.Role, x))).ToList();
            SymmetricSecurityKey sigingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            SigningCredentials credential = new SigningCredentials(sigingKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                configuration["jwt:issuer"],
                 configuration["jwt:audiance"],
                 claims,
                 expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["jwt:AccessTokenExpirationInMinutes"])),
                 signingCredentials: credential
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> generateRefreshToken(User user)
        {
            RefreshToken rToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(configuration["jwt:RefreshTokenExpirationInDays"])),
            };
            await context.RefreshTokens.AddAsync(rToken);
            await context.SaveChangesAsync();
            return rToken;
        }
    }
}
