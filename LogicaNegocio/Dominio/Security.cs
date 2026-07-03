using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio {
    public static class Security {
        public static string HashString(string password) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
        public static string HashearPasswordPBKDF2(string password) {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerificarPasswordPBKDF2(string password, string storedHash) {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                throw new InvalidDataException("El hash almacenado no tiene el formato correcto.");

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hashOriginal = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hashPrueba = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(hashOriginal, hashPrueba);
        }
    }
}
