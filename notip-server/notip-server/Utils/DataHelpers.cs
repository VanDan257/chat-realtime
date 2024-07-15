using System.Security.Cryptography;
using System.Text;

namespace notip_server.Utils
{
    public class DataHelpers
    {
        public static string HashSHA256(string data)
        {
            using (SHA256 hashSHA256 = SHA256.Create())
            {
                byte[] hashValue = hashSHA256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in hashValue)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static void Base64ToImage(string base64String, string filePath)
        {
            var bytes = Convert.FromBase64String(base64String);
            using (var imgFile = new FileStream(filePath, FileMode.Create))
            {
                imgFile.Write(bytes, 0, bytes.Length);
                imgFile.Flush();
            }
        }
    }
}
