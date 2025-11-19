namespace EventsProject.Domain.Models;

public partial class UserEvent {
    //-------------------------PROPERTIES-------------------------
    public int UserEventId { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public int TicketsAmount { get; set; }

    //-------------------------FG KEYS-------------------------
    public virtual EventInfo Event { get; set; } = null!;
    public virtual UserAccount User { get; set; } = null!;

    //-------------------------CTOR-------------------------
    public UserEvent() { }
    public UserEvent(int userId, int eventId, int tickets) {
        UserId = userId;
        EventId = eventId;
        TicketsAmount = tickets;
        EnrolledAt = DateTime.Now;
    }
}
