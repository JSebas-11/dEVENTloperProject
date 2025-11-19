using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions;

public interface IAppConfiguration {
    string GetConnection(string connName);
    UserAccount GetDefaultAdmin();
    SMTPConfig GetSmtpConfig();
}
