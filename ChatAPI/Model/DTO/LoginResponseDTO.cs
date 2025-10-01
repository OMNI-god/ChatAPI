namespace ChatAPI.Model.DTO
{
    public class LoginResponseDTO
    {
        public Guid UserId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public DateTime tokenExpiry { get; set; }
        public DateTime refreshTokenExpiry { get; set; }
        public string CsrfToken { get; set; }
    }
}
