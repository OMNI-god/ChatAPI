namespace ChatAPI.Middlewares
{
    public class TokenVerification
    {
        private readonly RequestDelegate _next;
        public TokenVerification(RequestDelegate _next)
        {
            this._next = _next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                // You have your JWT token string here
                // You can log it, validate it, or set claims
                Console.WriteLine($"Access token: {accessToken}");

                // Example: You could parse it manually
                // var handler = new JwtSecurityTokenHandler();
                // var jwtToken = handler.ReadJwtToken(accessToken);
            }

            await _next(context);
        }
    }
}
