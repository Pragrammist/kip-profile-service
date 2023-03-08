using System.Text;
using Core;
using System.Security.Cryptography;
using System.Text.Encodings;

namespace Infrastructure;

public class PasswordHasherImpl : PasswordHasher
{
    public async Task<string> Hash(string password)
    {
        using var mySHA256 = SHA256.Create();
        using var passwordStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        
        var hash = await mySHA256.ComputeHashAsync(passwordStream);

        return Encoding.UTF8.GetString(hash);
    }
}
