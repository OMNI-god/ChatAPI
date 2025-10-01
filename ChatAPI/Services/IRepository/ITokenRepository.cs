using System.Security.Claims;
using ChatAPI.Model.Domain;
using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Services.IRepository
{
    public interface ITokenRepository
    {
        (string, DateTime) generateJWTToken(User user, IEnumerable<string> roles);
        Task<(RefreshToken, DateTime)> generateRefreshToken(User user, string jwtToken, CancellationToken ct = default);
        Task<bool> validateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default);
        Task invalidateRefreshTokenAsync(Guid userID, string rawToken, CancellationToken ct = default);
        (bool isPresent, string refreshToken) isPresent();
        Task<RefreshToken> getRefreshTokenData(User user);
        (ClaimsPrincipal? Principal, DateTime? Expiry, string? RawToken) JwtValidator();
        Task<(RefreshToken refreshToken, DateTime expiry)> updateRefreshTokenData(RefreshToken refreshTokenData,string? jwtToken, bool IsAuthenticated, CancellationToken ct = default);
    }
}
