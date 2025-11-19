namespace EventsProject.Domain.Models;

public partial class UserAccount {
    //-------------------------PROPERTIES-------------------------
    public int UserId { get; set; }
    public string Dni { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string HashPassword { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public byte[]? UserImg { get; set; }
    public bool IsAdmin { get; set; }

    //-------------------------FK KEYS-------------------------
    public virtual ICollection<EventInfo> EventInfos { get; set; } = new List<EventInfo>();
    public virtual ICollection<NotificationInfo> NotificationInfos { get; set; } = new List<NotificationInfo>();
    public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

    //-------------------------CTORS-------------------------
    public UserAccount() { }
}
