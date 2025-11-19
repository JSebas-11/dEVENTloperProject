using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  gestionar los datos y la logica de negocio de las categorias
public interface ICategoryService {
    public Task<List<Category>> GetCategoriesAsync();
}
