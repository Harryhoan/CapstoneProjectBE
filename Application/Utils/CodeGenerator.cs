using System.Text;

namespace Application.Utils
{
    public static class CodeGenerator
    {
        private static readonly string AllowedCode = "0123456789";
        private static readonly Random Random = new();

        public static string GenerateRandomVerifyCode()
        {
            var sb = new StringBuilder();
            sb.Append((char)(Random.Next(1, 10) + '0'));
            for (int i = 1; i < 6; i++)
            {
                int index = Random.Next(10);
                sb.Append(AllowedCode[index]);
            }
            return sb.ToString();
        }
    }
}
