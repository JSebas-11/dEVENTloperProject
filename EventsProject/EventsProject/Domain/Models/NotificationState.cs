namespace EventsProject.Domain.Models;

public partial class NotificationState {
    //-------------------------PROPERTIES-------------------------
    public int NotStateId { get; set; }
    public string StateName { get; set; } = null!;

    //-------------------------FK KEYS-------------------------
    public virtual ICollection<NotificationInfo> NotificationInfos { get; set; } = new List<NotificationInfo>();
}
