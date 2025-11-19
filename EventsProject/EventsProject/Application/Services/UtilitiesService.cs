using EventsProject.Domain.Abstractions.Services;
using EventsProject.Utilities;

namespace EventsProject.Application.Services;

public class UtilitiesService : IUtilitiesService {
    //------------------------INITIALIZATION------------------------
    private readonly IHasher _hasher;
    public UtilitiesService(IHasher hasher) {
        _hasher = hasher;
    }

    //------------------------METHODS------------------------
    public string HashPassword(string password) => _hasher.HashPassword(password);
    public bool VerifyPassword(string password, string hashedPassword) => _hasher.VerifyPassword(password, hashedPassword);
}
