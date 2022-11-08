using Main.Services;

namespace Main.Defaults
{
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthService authService)
        {
            var sessionIdString = context.User.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;
            if (Guid.TryParse(sessionIdString, out var sessionId))
            {
                var session = await authService.GetSessionById(sessionId);
                if (!session.IsActive)
                {                    
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                    return;
                }
            }            
            await _next(context);
        }
    }
    public static class TokenValidatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidator(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidatorMiddleware>();
        }
    }
}
