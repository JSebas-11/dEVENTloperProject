using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para gestionar los datos
//y la logica de negocio de la entidad que relacion user-events
public interface IUserEventService {
    public Task<List<EventInfo>> GetUniqueEnrolledEventsByUserAsync(int userId, bool tracking = false);
    public Task<List<EventInfo>> GetLastNEventByUserAsync(int userId, int NumEvents = 5);
    public Task<List<EventInfo>> GetNextNEventByUserAsync(int userId, int NumEvents = 5);

    public int EnrrolledInEventCount(int eventId);
    public Task<int> TicketsBoughtByUserInAsync(int userId, int eventId);
    public Task<Result> EnrrollUserInAsync(int userId, int eventId, int amount);
}
