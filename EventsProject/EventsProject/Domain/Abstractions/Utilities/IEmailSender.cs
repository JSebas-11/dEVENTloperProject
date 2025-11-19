using EventsProject.Domain.Common;

namespace EventsProject.Domain.Abstractions.Utilities;

public interface IEmailSender {
    Task<Result> SendEmailAsync(string to, string subject, string body);
}
