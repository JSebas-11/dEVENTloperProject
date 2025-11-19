namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  operaciones adicionales en la UI
public interface IUtilitiesService {
    public string HashPassword(string password);
    public bool VerifyPassword(string password, string hashedPassword);
}
