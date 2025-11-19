using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;

namespace EventsProject.Application.Services;

public class EmailSenderService : IEmailSenderService {
    //------------------------INITIALIZATION------------------------
    private readonly IEmailSender _emailSender;
    private static readonly Random _random = new();
    public EmailSenderService(IEmailSender emailSender)
        => _emailSender = emailSender;

    //------------------------METHODS------------------------
    public async Task<Result> SendVerificationCodeAsync(string targetEmail) {
        //Generacion codigo
        int code = _random.Next(100000, 999999);

        string subject = "Recover Password Verification Code";
        string body = $@"
            <div style='font-family:Arial;'>
                <h2 style='color:#0078D7;'>Verification Code</h2>
                <p>Your code to restore your password is: 
                    <strong style='font-size:18px;color:#333;'>{code}</strong>
                </p>
            </div>";

        Result result = await _emailSender.SendEmailAsync(targetEmail, subject, body);

        //Adaptar mensaje ya que el de SendEmailAsync es generico
        if (result.Success)
            return Result.Ok(code.ToString());
        else
            return Result.Fail($"There has been an error sending verification code:\n{result.ExceptionMsg}", result.ExceptionMsg);
    }
}
