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

        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
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

        public RefreshToken generateRefreshToken()
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(configuration["jwt:RefreshTokenExpirationInDays"])),
                Created = DateTime.UtcNow
            };
        }
    }
}
