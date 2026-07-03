using Azure.Core;
using DTOs;
using LogicaAplicacion;
using LogicaNegocio.Dominio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Token
{
    public class ManejadorToken
    {

        public static string CrearToken(DTOUserLogged usu, JwtOptions jwtOptions)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            //clave secreta, generalmente se incluye en el archivo de configuración
            //Debe ser un vector de bytes 
            byte[] clave = Encoding.ASCII.GetBytes(jwtOptions.Key);
            //Se incluye un claim para el email
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Email,usu.Mail),
                    new Claim(ClaimTypes.Role, usu.Role)
                }),
                Expires = DateTime.UtcNow.AddSeconds(jwtOptions.TiempoVida),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(clave),
            SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static bool ValidarToken(string token, JwtOptions jwtOptions)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] clave = Encoding.ASCII.GetBytes(jwtOptions.Key);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(clave),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
    
