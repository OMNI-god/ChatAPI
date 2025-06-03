using ChatAPI.Model.DTO;

namespace ChatAPI.Services.IRepository
{
    public interface IUserRepository
    {
        ///<summary>
        /// User login method
        /// <description>
        /// Create token for existing user for access
        /// </description>
        /// <returns>
        /// LoginResponseDTO containing token, refresh token and user details
        /// </returns>
        ///</summary>
        Task<LoginResponseDTO> login(LoginRequestDTO loginRequestDTO);
        ///<summary>
        /// User register method
        /// <description>
        /// Register a new user
        /// </description>
        /// <returns>
        /// RegisterResponseDTO containing token, refresh token and user details
        /// </returns>
        ///</summary>
        Task<RegisterResponseDTO> register(RegisterRequestDTO registerRequestDTO);
    }
}
