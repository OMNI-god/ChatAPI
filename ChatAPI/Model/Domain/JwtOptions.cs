namespace ChatAPI.Model.Domain
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string[] Audience { get; set; } = Array.Empty<string>();
        public string Key { get; set; } = string.Empty;
        public int AccessTokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInDays { get; set; }
    }
}