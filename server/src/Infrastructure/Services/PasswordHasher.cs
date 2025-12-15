using System.Security.Cryptography;
using HrManager.Application.Abstractions.Services;

namespace HrManager.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
        var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        var salt = Convert.ToBase64String(algorithm.Salt);
        return $"v1.{Iterations}.{salt}.{key}";
    }

    public bool Verify(string hash, string password)
    {
        var segments = hash.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 4 || segments[0] != "v1")
        {
            return false;
        }

        var iterations = int.Parse(segments[1]);
        var salt = Convert.FromBase64String(segments[2]);
        var key = Convert.FromBase64String(segments[3]);

        using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var keyToCheck = algorithm.GetBytes(KeySize);
        return keyToCheck.SequenceEqual(key);
    }
}
