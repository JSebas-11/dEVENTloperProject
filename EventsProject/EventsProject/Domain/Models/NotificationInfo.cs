namespace EventsProject.Domain.Models;

public partial class NotificationInfo {
    public int NotificationId { get; set; }

    public int? NotStateId { get; set; }

    public int UserId { get; set; }

    public string NotMessage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual NotificationState? NotState { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
