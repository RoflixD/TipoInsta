using System.Text;
using System.Security.Cryptography;

namespace Coomon.Helpers
{
    public static class HashHelper
    {
        public static string GetHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var data = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (var c in data)
                {
                    sb.Append(c.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool Verify(string input, string hash)
        {
            var hashInput = GetHash(input);
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashInput, hash) == 0;
        }
    }
}
