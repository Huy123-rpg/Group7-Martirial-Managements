using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static string SHA256Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);

        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
