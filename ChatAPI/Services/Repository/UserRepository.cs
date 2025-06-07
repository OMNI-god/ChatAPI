using ChatAPI.Context;
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

        public UserRepository(UserManager<User> userManager,ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        public async Task<LoginResponseDTO> login(LoginRequestDTO loginRequestDTO)
        {
            User user=await userManager.Users.FirstOrDefaultAsync(x=>x.UserName==loginRequestDTO.userName_email||x.Email==loginRequestDTO.userName_email);
            if (user != null)
            {
                bool isValidPassword = await userManager.CheckPasswordAsync(user, loginRequestDTO.password);
                if (isValidPassword)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    string token = tokenRepository.generateJWTToken(user, roles.ToList());
                    return new LoginResponseDTO
                    {
                        userName = user.UserName,
                        email = user.Email,
                        token = token,
                        refreshToken = (await tokenRepository.generateRefreshToken(user)).Token,
                    };
                }
            }
            return null;
        }

        public async Task<RegisterResponseDTO> register(RegisterRequestDTO registerRequestDTO)
        {
            var user = new User
            {
                UserName = registerRequestDTO.userName,
                Email = registerRequestDTO.email
            };
            var identityResult=await userManager.CreateAsync(user, registerRequestDTO.password);
            if(identityResult.Succeeded)
            {
                if (registerRequestDTO.roles != null && registerRequestDTO.roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(user,registerRequestDTO.roles);
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
