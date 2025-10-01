using System.Security.Cryptography;

namespace Server.Utils
{
    public static class AuthUtils
    {
        public static string GenerateToken(int size)
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(size));
        }
    }
}
