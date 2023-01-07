namespace ProfileService.Core;

//TODO: Exceptions if will needed

public class User
{

    private User() { }
    void Constructore(string login, string email, string password)
    {
        Login = login;
        Email = email;
        Password = password;
    }
    public User(string login, string email, string password)
    {
        Constructore(login, email, password);
    }

    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}
