using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PasswordHash: IPasswordHash
    {
        public string Encrypt(string password, out string salt)
        {
            byte[] saltBytes = new byte[16];

            RandomNumberGenerator.Fill(saltBytes);

            salt = Convert.ToBase64String(saltBytes);

            const int iterations = 100_000;
            const int hashLength = 32;

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(hashLength);
                return Convert.ToBase64String(hashBytes);
            }
        }
        public bool Decrypt(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            const int iterations = 100000;
            const int hashLength = 32;

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(hashLength);
                string computedHash = Convert.ToBase64String(hashBytes);

                return computedHash == storedHash;
            }
        }
    }
}
