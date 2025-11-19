using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  gestionar los datos y la logica de negocio de los estados de evento
public interface IEventStateService {
    public Task<List<EventState>> GetEventStatesAsync();
}
