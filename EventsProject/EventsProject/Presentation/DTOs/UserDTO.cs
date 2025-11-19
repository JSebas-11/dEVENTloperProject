using System.Windows.Media.Imaging;

namespace EventsProject.Presentation.DTOs;

//Clase para inyectar en las Pages con el objeto BitMapImage para facilitar
//la visualizacion de las imagenes de usuarios
public class UserDTO {
    //-------------------------PROPERTIES-------------------------
    public int UserId { get; set; }
    public string Dni { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string HashPassword { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public BitmapImage? UserImg { get; set; }
}