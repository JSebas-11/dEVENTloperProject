namespace EventsProject.Domain.Models;

public partial class EventState {
    //-------------------------PROPERTIES-------------------------
    public int EventStateId { get; set; }
    public string StateName { get; set; } = null!;

    //-------------------------FK KEYS-------------------------
    public virtual ICollection<EventInfo> EventInfos { get; set; } = new List<EventInfo>();
}
