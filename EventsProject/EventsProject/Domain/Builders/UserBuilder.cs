using EventsProject.Domain.Models;

namespace EventsProject.Domain.Builders;

public class UserBuilder {
    //-------------------------INITIALIZATION-------------------------
    private UserAccount _user;

    public UserBuilder() => _user = new UserAccount();

    //-------------------------METHODS-------------------------
    public UserAccount Build() => _user;

    //-------------------------propsMeths-------------------------
    public UserBuilder WithGeneralInfo(string dni, string email, string userName) {
        _user.Dni = dni.Trim();
        _user.UserEmail = email.Trim();
        _user.UserName = userName.Trim();

        return this;
    }
    
    public UserBuilder WithPassword(string hashPassword) {
        _user.HashPassword = hashPassword;

        return this;
    }
    
    public UserBuilder IsAdmin(bool isAdmin) {
        _user.IsAdmin = isAdmin;

        return this;
    }

    public UserBuilder WithUserImg(byte[]? userImg = null) {
        _user.UserImg = userImg;

        return this;
    }

}
