using System.Security.Cryptography;
using System.Text;

namespace Application.Utils
{
    public class HashPassWithSHA256
    {
        public static string HashWithSHA256(string input)
        {
            using SHA256 sHA256 = SHA256.Create();

            var inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] bytes = sHA256.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
        public static string AsHmacSHA512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);

            using var hmac = new HMACSHA512(keyBytes);
            return BitConverter.ToString(hmac.ComputeHash(inputBytes)).Replace("-", string.Empty);
        }
    }
}
