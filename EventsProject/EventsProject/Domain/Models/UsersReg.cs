namespace EventsProject.Domain.Models;

public partial class UsersReg
{
    public int UsersRegId { get; set; }

    public string Dni { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
