using notip_server.Interfaces;
using System.Security.Cryptography;

namespace notip_server.Service
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256); // Sử dụng PBKDF2 với SHA-256 để băm mật khẩu cùng với salt được cung cấp.
            byte[] hash = pbkdf2.GetBytes(20);
            return Convert.ToBase64String(hash);
        }

        public string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public bool VerifyPassword(string hashedPassword, string salt, string password)
        {
            string hashedEnteredPassword = HashPassword(password, salt);
            return hashedPassword == hashedEnteredPassword;
        }
    }
}
