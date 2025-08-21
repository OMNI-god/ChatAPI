using ChatAPI.Model.Domain;
using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Services.IRepository
{
    public interface ITokenRepository
    {
        (string, DateTime) generateJWTToken(User user, IEnumerable<string> roles);
        Task<(RefreshToken, DateTime)> generateRefreshToken(User user, CancellationToken ct = default);
        Task<bool> validateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default);
        Task invalidateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default);
    }
}
