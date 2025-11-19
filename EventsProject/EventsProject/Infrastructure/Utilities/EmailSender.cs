using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace EventsProject.Infrastructure.Utilities;

public class EmailSender : IEmailSender {
    //------------------------INITIALIZATION------------------------
    private readonly SMTPConfig _smtpConfig;
    public EmailSender(SMTPConfig smtpConfig)
        => _smtpConfig = smtpConfig;

    //------------------------METHODS------------------------
    public async Task<Result> SendEmailAsync(string to, string subject, string body) {
        try {
            var msg = new MimeMessage();

            //Agregar datos del mensaje del correo
            msg.From.Add(new MailboxAddress("dEVENTloper HeadQuarters", _smtpConfig.Email));
            msg.To.Add(new MailboxAddress("User", to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = body };

            //Conectar el SMTPClient y enviar
            await SendBySmtpClient(msg);

            return Result.Ok("Message was sent successfully, take a look at your email");
        }
        catch (Exception ex) {
            return Result.Fail($"There has been an error sending email", ex.Message);
        }
    }

    //------------------------innerMeths------------------------
    private async Task SendBySmtpClient(MimeMessage message) {
        using (var client = new SmtpClient()) {
            //Conectarse al cliente de acuerdo al SMTPConfig (data proviene del appsettings.json)
            await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, _smtpConfig.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_smtpConfig.Email, _smtpConfig.Password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
