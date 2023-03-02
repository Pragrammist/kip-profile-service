

namespace ProfileService.Core;

//TODO: Exceptions if will needed

public class User
{

    private User() { }
    void Constructor(string login, string email, string password)
    {
        
        Login = login;
        Email = email;
        Password = password;

        if(string.IsNullOrWhiteSpace(Login))
            throw new FieldIsNullOrEmptyException(nameof(Login), nameof(User));
        if(string.IsNullOrWhiteSpace(Email))
            throw new FieldIsNullOrEmptyException(nameof(Email), nameof(User));
        if(string.IsNullOrWhiteSpace(Password))
            throw new FieldIsNullOrEmptyException(nameof(Password), nameof(User));
    }
    public User(string login, string email, string password)
    {
        Constructor(login, email, password);
    }

    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}
