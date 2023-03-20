namespace Core;

public interface EmailSender
{   
    Task SendCode(string code, string email, CancellationToken token);
}