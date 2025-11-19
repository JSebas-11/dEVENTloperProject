using EventsProject.Domain.Common;
using System.Windows.Media.Imaging;

namespace EventsProject.Presentation.DTOs;

//Clase usada en page UserEvent para facilitar la visualizacion de los eventos y suscripcion con los mismos
public class EventDTO {
    //-------------------------PROPERTIES-------------------------
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public string EventPlace { get; set; } = null!;
    public string EventCity { get; set; } = null!;
    public DateTime InitialTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Artist { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public string? Category { get; set; }
    public int Capacity { get; set; }
    public EnumEventState EventState { get; set; }
    public BitmapImage? EventImg { get; set; }
}