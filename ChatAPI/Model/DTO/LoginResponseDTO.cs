namespace ChatAPI.Model.DTO
{
    public class LoginResponseDTO
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
