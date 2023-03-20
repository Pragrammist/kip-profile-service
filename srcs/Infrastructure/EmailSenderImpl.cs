using System.Net;
using System.Net.Mail;
using Core;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class EmailSenderOptions
{
    public string SmtpEmailCrend { get; set; } = null!;
    public string SmtpPasswordCrend { get; set; } = null!;
    public string SmtpServer { get; set; } = null!;
    public int SmtpPort { get; set; }
}
public class EmailSenderImpl : EmailSender
{
    readonly EmailSenderOptions _options;
    public EmailSenderImpl(EmailSenderOptions options)
    {
        _options = options;
    }
    public async Task SendCode(string code, string email, CancellationToken token)
    {
        var from = new MailAddress(_options.SmtpEmailCrend, "КИП");
            // кому отправляем
        var to = new MailAddress(email);
        // создаем объект сообщения
        var m = new MailMessage(from, to)
        {
            // тема письма
            Subject = "Код восстановления пароля от КИП",
            // текст письма
            Body = @$"
                        <h1>{code}</h1>
                        <h2>Если вы не подавали заявку на восстановления пароля, то игнорируйте это сообщение</h2>
                        <p>Если вы переживаете за свой аккаунт, то поставьте сложный пароль, если он у вас простой</p>
                    ",
            // письмо представляет код html
            IsBodyHtml = true
        };
        // адрес smtp-сервера и порт, с которого будем отправлять письмо
        var smtp = new SmtpClient(_options.SmtpServer, _options.SmtpPort)
        {
            // логин и пароль
            Credentials = new NetworkCredential(_options.SmtpEmailCrend, _options.SmtpPasswordCrend),
            EnableSsl = true,
            
        };
        await smtp.SendMailAsync(m, token);
    }

    
}