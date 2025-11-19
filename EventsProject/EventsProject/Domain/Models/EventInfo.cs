namespace EventsProject.Domain.Models;

public partial class EventInfo {
    //-------------------------PROPERTIES-------------------------
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string EventPlace { get; set; } = null!;
    public string EventCity { get; set; } = null!;
    public int? CatId { get; set; }
    public DateTime InitialTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MinutesDuration { get; set; }
    public string Artist { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public int Capacity { get; set; }
    public int? EventStateId { get; set; }
    public byte[]? EventImg { get; set; }
    public int? CreatedById { get; set; }

    //-------------------------FK KEYS-------------------------
    public virtual Category? Cat { get; set; }
    public virtual UserAccount? CreatedBy { get; set; }
    public virtual EventState? EventState { get; set; }
    public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

    //-------------------------CTOR-------------------------
    public EventInfo() { }
}
