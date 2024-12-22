using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalR_Test.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SignalR_Test.Token
{
    public class TokenService
    {
        private readonly string secKey;
        private IConfiguration configuration;
        private readonly SymmetricSecurityKey secretKey;
        private readonly SigningCredentials signingCredentials;
        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
            secKey = configuration["jwt:key"];
            secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            signingCredentials=new SigningCredentials(secretKey,SecurityAlgorithms.HmacSha256);
        }
        public string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim("username",user.Username),
                new Claim("id",user.Id.ToString()),
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["jwt:AccessTokenExpirationInMinutes"])),
                SigningCredentials = signingCredentials,
                Issuer = configuration["jwt:issuer"],
                Audience = configuration["jwt:audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
