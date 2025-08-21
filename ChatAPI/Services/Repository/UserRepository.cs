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

        public UserRepository(UserManager<User> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        public async Task<LoginResponseDTO> login(LoginRequestDTO loginRequestDTO, CancellationToken ct = default)
        {
            User user = await userManager.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == loginRequestDTO.userName_email || x.Email == loginRequestDTO.userName_email);
            if (user != null)
            {
                bool isValidPassword = await userManager.CheckPasswordAsync(user, loginRequestDTO.password);
                if (isValidPassword)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    (string token, DateTime tokenExpiry) = tokenRepository.generateJWTToken(user, roles.ToList());
                    (RefreshToken refreshToken, DateTime refreshTokenExpiry) = await tokenRepository.generateRefreshToken(user);
                    return new LoginResponseDTO
                    {
                        Id = user.Id,
                        userName = user.UserName,
                        email = user.Email,
                        token = token,
                        tokenExpiry = tokenExpiry,
                        refreshToken = refreshToken.RawToken,
                        refreshTokenExpiry = refreshTokenExpiry,
                    };
                }
            }
            return null;
        }

        public async Task<RegisterResponseDTO> register(RegisterRequestDTO registerRequestDTO, CancellationToken ct = default)
        {
            var user = new User
            {
                UserName = registerRequestDTO.userName,
                Email = registerRequestDTO.email
            };
            var identityResult = await userManager.CreateAsync(user, registerRequestDTO.password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDTO.roles != null && registerRequestDTO.roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(user, registerRequestDTO.roles);
                    if (identityResult.Succeeded)
                    {
                        return new RegisterResponseDTO
                        {
                            response = "User registered successfully.",
                        };
                    }
                }
                else
                {
                    identityResult = await userManager.AddToRoleAsync(user, "User");
                    if (identityResult.Succeeded)
                    {
                        return new RegisterResponseDTO
                        {
                            response = "User registered successfully.",
                        };
                    }
                }
            }

            return new RegisterResponseDTO
            {
                response = string.Join(",", identityResult.Errors.Select(e => e.Description))
            };
        }
    }
}
