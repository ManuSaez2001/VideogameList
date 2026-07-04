using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                    _logger.LogWarning("Token expirado o inválido, eliminando cookie");

                    // Eliminar la cookie del token
                    context.Response.Cookies.Delete("JWTToken");
                }
                else
                {
                    // Extraer los claims del token válido y establecer como principal
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);
                        var claims = jwtToken.Claims.ToList();

                        // Logging de claims para debugging
                        var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                        _logger.LogInformation($"Token validado para usuario: {emailClaim}, Rol: {roleClaim}");

                        // Crear la identidad con los claims del JWT usando el esquema de cookies
                        // Esto es lo que permite que [Authorize] funcione
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Establecer el usuario en el contexto
                        var principal = new ClaimsPrincipal(identity);
                        context.User = principal;

                        _logger.LogInformation($"Usuario establecido: {context.User.Identity?.IsAuthenticated}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error al procesar el token: {ex.Message}");
                        context.Response.Cookies.Delete("JWTToken");
                    }
                }
            }

            await _next(context);
        }
    }
}
