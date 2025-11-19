namespace EventsProject.Domain.Abstractions.Repositories;

//Interface para entidades que no tendran mas instancias solo se usaran
//las predefinidas en el diseño de la DB en SQLSERVER
public interface IReadOnlyRepository<T> {
    IQueryable<T> GetAll();
    Task<T?> GetByIdAsync(int modelId);
}
