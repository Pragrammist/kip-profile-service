namespace Appservices;

public interface PasswordHasher
{
    Task<string> Hash(string password);
}
