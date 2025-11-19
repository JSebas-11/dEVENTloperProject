using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  gestionar los datos y la logica de negocio de los eventos
public interface IEventService {
    public Task<List<EventInfo>> GetEventsAsync();
    public Task<List<EventInfo>> GetEventsCreatedByIdAsync(int userId);
    public Task<List<EventInfo>> GetEventsWithCartegoriesAsync();
    public Task<Result> CreateEventAsync(
        string title, int capacity, int catId, string artist, string eventDescription,
        DateTime initialTime, DateTime endTime,
        string eventPlace, string eventCity,
        int userId, bool isAdmin, string? eventImgPath = null
    );
    public Task<Result> UpdateEventAsync(EventInfo eventInfo);
    public Result ValidateEventFields(
        bool editValidation, string? title, int? capacity,
        int? catId, string? artist, string? eventDescription,
        DateTime? initialTime, DateTime? endTime,
        string? eventPlace, string? eventCity, int? stateId = null
    );

    public Task<bool> IsSpotAvailableAsync(string eventCity, string eventPlace, DateTime initTime, DateTime endTime);
    public Task<Result> ChangeEventStateAsync(EventInfo originalEvent, int newEventStateId);
    public Task<Result> UpdatePastEventsAsync();
}
