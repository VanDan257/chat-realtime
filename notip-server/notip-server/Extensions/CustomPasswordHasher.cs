using Microsoft.AspNetCore.Identity;
using notip_server.Models;
using System.Security.Cryptography;
using System.Text;

namespace notip_server.Extensions
{
    public class CustomPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : User
    {
        public string HashPassword(TUser user, string password)
        {
            if (user.PasswordSalt == null)
            {
                user.PasswordSalt = GenerateSalt();
            }

            byte[] saltBytes = Convert.FromBase64String(user.PasswordSalt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20); // 20 bytes for SHA-1
            return Convert.ToBase64String(hash);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            var providedPasswordHash = HashPassword(user, providedPassword);
            if (hashedPassword == providedPasswordHash)
            {
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
    }

}
