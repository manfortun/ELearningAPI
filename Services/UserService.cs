using eLearningApi.DataAccess;
using eLearningApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace eLearningApi.Services;

public abstract class UserService
{
    /// <summary>
    /// Checks if a <paramref name="candidateEmailAddress"/> is already existing in the database
    /// </summary>
    /// <param name="candidateEmailAddress"></param>
    /// <returns></returns>
    public static bool CanRegisterEmail(BaseRepository<User> users, string candidateEmailAddress)
    {
        string lowered = candidateEmailAddress.ToLower();
        IEnumerable<User> existingUser = users.Get(u => u.Email.ToLower() == lowered);

        return existingUser?.Any() != true;
    }

    /// <summary>
    /// Sets the salt and password of the <paramref name="dto"/>.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static void SetUserSecurity(User dto)
    {
        ArgumentException.ThrowIfNullOrEmpty(dto.Password);

        byte[] salt = GenerateSalt();
        byte[] password = Encoding.UTF8.GetBytes(dto.Password);

        // combine salt and password bytes
        byte[] combinedBytes = CombineByteArray(salt, password);

        using (var sha512 = SHA512.Create())
        {
            byte[] hashedBytes = sha512.ComputeHash(combinedBytes);

            dto.Salt = Convert.ToBase64String(salt);
            dto.Password = Convert.ToBase64String(hashedBytes);
        }
    }


    /// <summary>
    /// Generates salt.
    /// </summary>
    public static byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }

    public static byte[] CombineByteArray(byte[] first, byte[] second)
    {
        byte[] combined = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, combined, 0, first.Length);
        Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);

        return combined;
    }

    /// <summary>
    /// Checks if the password of <paramref name="user"/> is equal to <paramref name="password"/>.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool VerifyPassword(User user, string password)
    {
        byte[] salt = Convert.FromBase64String(user.Salt);

        using (var sha512 = SHA512.Create())
        {
            byte[] combined = CombineByteArray(salt, Encoding.UTF8.GetBytes(password));
            byte[] hashedBytes = sha512.ComputeHash(combined);

            string hashedEnteredPassword = Convert.ToBase64String(hashedBytes);

            return hashedEnteredPassword == user.Password;
        }
    }
}
