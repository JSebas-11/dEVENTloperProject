namespace EventsProject.Domain.Models;

public partial class Category {
    //-------------------------PROPERTIES-------------------------
    public int CatId { get; set; }
    public string CatName { get; set; } = null!;
    public string CatDescription { get; set; } = null!;

    //-------------------------FK KEYS-------------------------
    public virtual ICollection<EventInfo> EventInfos { get; set; } = new List<EventInfo>();
}
