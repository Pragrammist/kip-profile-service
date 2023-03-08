namespace Core;

public interface PasswordHasher
{
    Task<string> Hash(string password);
}
