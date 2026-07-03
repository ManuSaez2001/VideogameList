using WebAPI.Token;

namespace VideogameListMVC.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtValidationMiddleware> _logger;
        private readonly JwtOptions _jwtOptions;

        public JwtValidationMiddleware(RequestDelegate next, ILogger<JwtValidationMiddleware> logger, JwtOptions jwtOptions)
        {
            _next = next;
            _logger = logger;
            _jwtOptions = jwtOptions;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Obtener el token de la cookie
            if (context.Request.Cookies.TryGetValue("JWTToken", out var token))
            {
                // Validar el token
                if (!WebAPI.Token.ManejadorToken.ValidarToken(token, _jwtOptions))
                {
                    _logger.LogWarning("Token expirado o inválido, eliminando cookie y redirigiendo a login");

                    // Eliminar la cookie del token
                    context.Response.Cookies.Delete("JWTToken");

                    // Redirigir a User/Login
                    context.Response.Redirect("/User/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}
