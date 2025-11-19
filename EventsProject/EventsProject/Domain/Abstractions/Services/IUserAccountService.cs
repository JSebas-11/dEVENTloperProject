using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  gestionar los datos y la logica de negocio de los usuarios
public interface IUserAccountService {
    public Task<bool> ExistsUserAsync(string dni);
    public Task<string?> GetUserEmailAsync(string dni);
    public Task<UserAccount?> GetUserAsync(string dni);
    public Task<UserAccount?> GetUserAsync(string dni, string password);
    public Result ValidateUserFields(string? dni, string? email, string? userName, string? password);
    public Task<Result> CreateUserAsync(
        string dni, string email, string userName, string password, 
        string? userImgPath = null, bool isAdmin = false
    );
    public Task<Result> UpdateUserAsync(UserAccount userAccount);
}
