using System.Security.Cryptography;
using System.Text;

namespace CardStorageService.Utils;

public static class PasswordUtils
{
    private const string SecretKey = "111111111111111111111111111111";

    public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password)
    {
        string passwordSalt = GenerateRandomSalt();
        string passwordHash = GetPasswordHash(password, passwordSalt);

        return (passwordSalt, passwordHash);
    }

    public static string GetPasswordHash(string password, string passwordSalt)
    {
        string combinedString = $"{password}~{passwordSalt}~{SecretKey}";
        byte[] buffer = Encoding.UTF8.GetBytes(combinedString);

        SHA512 sha512 = SHA512.Create();
        byte[] passwordHash = sha512.ComputeHash(buffer);

        return Convert.ToBase64String(passwordHash);
    }

    public static bool VerifyPassword(string password, string passwordSalt, string passwordHash)
    {
        return passwordHash == GetPasswordHash(password, passwordSalt);
    }
    private static string GenerateRandomSalt()
    {
        // RNGCryptoServiceProvider is obsolete
        int count = 16;
        byte[] buffer = RandomNumberGenerator.GetBytes(count);
        return Convert.ToBase64String(buffer);
    }
}
