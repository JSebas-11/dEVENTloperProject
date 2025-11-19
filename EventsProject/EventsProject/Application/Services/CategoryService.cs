using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Application.Services;

public class CategoryService : ICategoryService {
    //------------------------INITIALIZATION------------------------
    private readonly IReadOnlyRepository<Category> _categoryRepository;
    public CategoryService(IReadOnlyRepository<Category> categoryRepository) {
        _categoryRepository = categoryRepository;
    }

    //------------------------METHODS------------------------
    public async Task<List<Category>> GetCategoriesAsync() 
        => await _categoryRepository.GetAll().ToListAsync();
}
