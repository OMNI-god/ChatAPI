using ChatAPI.Model.Domain;
using ChatAPI.Model.DTO;
using ChatAPI.Services.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(UserManager<User> userManager, ITokenRepository tokenRepository, ILogger<UserRepository> logger)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.logger = logger;
        }
        public async Task<LoginResponseDTO> login(LoginRequestDTO loginRequestDTO, CancellationToken ct = default)
        {
            try
            {
                logger.LogInformation("Login attempt for {User}", loginRequestDTO.userName_email);

                var user = await userManager.Users.AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.UserName == loginRequestDTO.userName_email || x.Email == loginRequestDTO.userName_email,
                        ct);

                if (user == null)
                {
                    logger.LogWarning("Login failed. User {User} not found.", loginRequestDTO.userName_email);
                    return null;
                }

                if (!await userManager.CheckPasswordAsync(user, loginRequestDTO.password))
                {
                    logger.LogWarning("Login failed. Invalid password for user {User}.", loginRequestDTO.userName_email);
                    return null;
                }

                var roles = await userManager.GetRolesAsync(user);
                var refreshTokenResult = tokenRepository.isPresent();
                var refreshTokenDBData = await tokenRepository.getRefreshTokenData(user);

                // Handle returning or refreshing tokens
                if (refreshTokenResult.isPresent && refreshTokenDBData != null)
                {
                    return await HandleRefreshToken(user, roles, refreshTokenResult.refreshToken, refreshTokenDBData, ct);
                }

                // New login (no prior refresh token)
                return await GenerateNewLoginResponse(user, roles, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred during login for {User}", loginRequestDTO.userName_email);
                throw; // Let middleware handle consistent error responses
            }
        }

        private async Task<LoginResponseDTO> HandleRefreshToken(User user, IList<string> roles, string existingRefreshToken, RefreshToken refreshTokenDBData, CancellationToken ct)
        {
            var result = tokenRepository.JwtValidator();

            if (!refreshTokenDBData.IsExpired)
            {
                if (result.Principal?.Identity?.IsAuthenticated == true)
                {
                    logger.LogInformation("User {User} logged in with valid existing JWT + Refresh token.", user.UserName);
                    return BuildLoginResponse(user, result.RawToken!, result.Expiry!.Value, existingRefreshToken, refreshTokenDBData.Expires);
                }

                // JWT invalid but refresh token valid → generate new JWT
                var (token, expiry) = tokenRepository.generateJWTToken(user, roles);
                logger.LogInformation("User {User} issued new JWT (reusing valid refresh token).", user.UserName);
                return BuildLoginResponse(user, token, expiry, existingRefreshToken, refreshTokenDBData.Expires);
            }

            // Refresh token expired → update DB
            if (result.Principal?.Identity?.IsAuthenticated == true)
            {
                var updateResult = await tokenRepository.updateRefreshTokenData(refreshTokenDBData, result.RawToken, true, ct);
                logger.LogInformation("User {User} updated expired refresh token.", user.UserName);
                return BuildLoginResponse(user, result.RawToken!, result.Expiry!.Value, updateResult.refreshToken.RawToken, updateResult.expiry);
            }
            else
            {
                var (token, expiry) = tokenRepository.generateJWTToken(user, roles);
                var updateResult = await tokenRepository.updateRefreshTokenData(refreshTokenDBData, result.RawToken, false, ct);
                logger.LogInformation("User {User} generated new JWT + updated expired refresh token.", user.UserName);
                return BuildLoginResponse(user, token, expiry, updateResult.refreshToken.RawToken, updateResult.expiry);
            }
        }

        private async Task<LoginResponseDTO> GenerateNewLoginResponse(User user, IList<string> roles, CancellationToken ct)
        {
            var (token, tokenExpiry) = tokenRepository.generateJWTToken(user, roles);
            var (refreshToken, refreshTokenExpiry) = await tokenRepository.generateRefreshToken(user, token, ct);

            logger.LogInformation("User {User} logged in for the first time. Issued JWT + Refresh token.", user.UserName);

            return BuildLoginResponse(user, token, tokenExpiry, refreshToken.RawToken, refreshTokenExpiry);
        }

        private static LoginResponseDTO BuildLoginResponse(User user, string token, DateTime tokenExpiry, string refreshToken, DateTime refreshTokenExpiry)
        {
            return new LoginResponseDTO
            {
                Id = user.Id,
                userName = user.UserName,
                email = user.Email,
                token = token,
                tokenExpiry = tokenExpiry,
                refreshToken = refreshToken,
                refreshTokenExpiry = refreshTokenExpiry,
            };
        }

        public async Task<RegisterResponseDTO> register(RegisterRequestDTO registerRequestDTO, CancellationToken ct = default)
        {
            try
            {
                var user = new User
                {
                    UserName = registerRequestDTO.userName,
                    Email = registerRequestDTO.email
                };

                var identityResult = await userManager.CreateAsync(user, registerRequestDTO.password);

                if (!identityResult.Succeeded)
                {
                    logger.LogWarning("User registration failed for {Email}. Errors: {Errors}",
                        registerRequestDTO.email,
                        string.Join(", ", identityResult.Errors.Select(e => e.Description)));

                    return new RegisterResponseDTO
                    {
                        response = string.Join(",", identityResult.Errors.Select(e => e.Description))
                    };
                }

                if (registerRequestDTO.roles != null && registerRequestDTO.roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(user, registerRequestDTO.roles);
                }
                else
                {
                    identityResult = await userManager.AddToRoleAsync(user, "User");
                }

                if (identityResult.Succeeded)
                {
                    logger.LogInformation("User {Email} registered successfully with roles: {Roles}", user.Email, string.Join(",", registerRequestDTO.roles ?? new[] { "User" }));
                    return new RegisterResponseDTO { response = "User registered successfully." };
                }

                return new RegisterResponseDTO
                {
                    response = string.Join(",", identityResult.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred during registration for {Email}", registerRequestDTO.email);
                throw;
            }
        }
    }
}
