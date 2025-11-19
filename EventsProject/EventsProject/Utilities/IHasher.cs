namespace EventsProject.Utilities;

//Interface de hasheo de contraseñas para inyectar en DataService
public interface IHasher {
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}