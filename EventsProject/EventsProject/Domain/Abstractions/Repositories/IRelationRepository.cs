using EventsProject.Domain.Common;

namespace EventsProject.Domain.Abstractions.Repositories;

//Interface para centralizar la persistencia y el acceso a datos de los
//modelos que actuan como puente de entidades (UserEvents)
public interface IUserEventRepository<T> {
    IQueryable<T> GetAll(bool tracking = false);
    IQueryable<T> GetEventsByUserId(int userId, bool tracking = false);
    IQueryable<T> GetUsersByEventId(int eventId, bool tracking = false);
    Task<T?> GetByIdsAsync(int userId, int eventId);
    Task<Result> InsertAsync(T model);
    Task<Result> UpdateAsync(T model);
    Task<Result> DeleteAsync(int userId, int eventId);
}
