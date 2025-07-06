using ChatAPI.Model.Domain;
using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Services.IRepository
{
    public interface ITokenRepository
    {
        (string,DateTime) generateJWTToken(User user, List<string> roles);
        Task<(RefreshToken,DateTime)> generateRefreshToken(User user);
    }
}
