using EventsProject.Domain.Common;

namespace EventsProject.Infrastructure;

public static class Config {
    //Lista con los posibles estados de eventos
    public static readonly List<EnumEventState> eventStates = [
        EnumEventState.Active, EnumEventState.Pending, EnumEventState.Cancelled, 
        EnumEventState.Finished, EnumEventState.SoldOut
    ];

    //Array con los formatos validos para las imagenes a subir en la DB
    public static readonly List<string> validExtensions = [".png", ".jpg", ".jpeg"];
    //Tamaño maximo de las imagenes (5MB)
    public const long maxSizeBytes = 5 * 1024 * 1024;
}
