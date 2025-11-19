using EventsProject.Utilities;

namespace EventsProject.Infrastructure.Utilities;

public class PasswordHasher : IHasher {
    public PasswordHasher() {}

    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}
