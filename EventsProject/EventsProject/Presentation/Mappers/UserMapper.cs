using EventsProject.Domain.Models;
using EventsProject.Presentation.DTOs;
using EventsProject.Properties;
using EventsProject.Utilities;

namespace EventsProject.Presentation.Mappers;

public static class UserMapper {
    public static UserDTO ToDTO(UserAccount user, IImgConvert imgConverter) {
        return new UserDTO() {
            UserId = user.UserId,
            Dni = user.Dni,
            UserName = user.UserName,
            HashPassword = user.HashPassword,
            UserEmail = user.UserEmail,
            IsAdmin = user.IsAdmin,
            UserImg = user.UserImg == null ? imgConverter.BinToImg(Resources.defaultUserIMG)
                : imgConverter.BinToImg(user.UserImg)
        };
    }
}
