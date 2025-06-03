using ChatAPI.Model.Domain;
using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Services.IRepository
{
    public interface ITokenRepository
    {
        string generateJWTToken(User user, List<string> roles);
        RefreshToken generateRefreshToken();
    }
}
