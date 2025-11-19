using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Builders;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EventsProject.Application.Services;

public class UserAccountService : IUserAccountService {
    //------------------------INITIALIZATION------------------------
    private readonly IRepository<UserAccount> _userAccountRepository;
    private readonly IHasher _hasher;
    private readonly IImgConvert _imgConverter;

    public UserAccountService(IRepository<UserAccount> userAccountRepository, IHasher hasher, IImgConvert imgConverter) {
        _userAccountRepository = userAccountRepository;
        _hasher = hasher;
        _imgConverter = imgConverter;
    }

    //------------------------METHODS------------------------
    public async Task<bool> ExistsUserAsync(string dni) => await _userAccountRepository.GetAll().AnyAsync(u => u.Dni == dni);
    public async Task<string?> GetUserEmailAsync(string dni) { 
        UserAccount? user = await _userAccountRepository.GetAll().FirstOrDefaultAsync(u => u.Dni == dni);
        
        return user?.UserEmail;
    }
    public async Task<UserAccount?> GetUserAsync(string dni) 
        => await _userAccountRepository.GetAll().FirstOrDefaultAsync(u => u.Dni == dni);
    public async Task<UserAccount?> GetUserAsync(string dni, string password) {
        //Obtener usuario y comprobar que exista
        UserAccount? user = await _userAccountRepository.GetAll().FirstOrDefaultAsync(u => u.Dni == dni);
        if (user == null)
            return null;

        //Retornar usuario en caso de coincidir las contraseñas, sino null
        return _hasher.VerifyPassword(password, user.HashPassword) ? user : null;
    }

    public Result ValidateUserFields(string? dni, string? email, string? userName, string? password) {
        //String para ir acumulando errores
        var errors = new StringBuilder();

        ValidationHelper.ValidateLength(dni, ValidationConsts.MinDniLength, ValidationConsts.MaxDniLength, "DNI", errors);
        ValidationHelper.ValidateLength(userName, ValidationConsts.MinUserNameLength, ValidationConsts.MaxUserNameLength, "UserName", errors);

        ValidationHelper.ValidateNull(email, "Email", errors);
        if (email is not null)
            ValidationHelper.ValidateEmail(email, errors);
        
        ValidationHelper.ValidateNull(password, "Password", errors);
        if (password is not null)
            ValidationHelper.ValidatePassword(password, errors);

        //Devolver el result con el mensaje y resultado correpondiente
        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }

    public async Task<Result> CreateUserAsync(string dni, string email, string userName, string password, string? userImgPath = null, bool isAdmin = false) {
        //Metodo para convertir de path a binario con su comprobacion
        byte[]? imgBin = null;
        if (userImgPath != null) {
            //Valida formato, tamaño y que archivo exista
            Result validation = _imgConverter.ValidImgPath(userImgPath);
            if (!validation.Success)
                return validation;

            //En caso de tener cumplir todos los requisitos los transformamos a binario
            imgBin = await _imgConverter.ImgPathToBinAsync(userImgPath);
        }

        //Creacion del objeto mediante su clase Builder
        UserAccount user = new UserBuilder()
            .WithGeneralInfo(dni, email, userName)
            .WithPassword(_hasher.HashPassword(password))
            .IsAdmin(isAdmin)
            .WithUserImg(imgBin)
            .Build();

        return await _userAccountRepository.InsertAsync(user);
    }

    public async Task<Result> UpdateUserAsync(UserAccount userAccount) => await _userAccountRepository.UpdateAsync(userAccount);
}
