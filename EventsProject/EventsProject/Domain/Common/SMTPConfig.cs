namespace EventsProject.Domain.Common;

public record SMTPConfig(
    string? Email, string? Password, string? Host, int Port, bool EnableSsl
);
