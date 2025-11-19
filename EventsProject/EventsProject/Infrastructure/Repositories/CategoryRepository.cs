using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Infrastructure.Repositories;

public class CategoryRepository : IReadOnlyRepository<Category> {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;

    public CategoryRepository(EventsProjectContext context) { _context = context; }

    //------------------------METHODS------------------------
    public IQueryable<Category> GetAll() 
        => _context.Categories.AsNoTracking();
    public async Task<Category?> GetByIdAsync(int modelId) 
        => await _context.Categories.FindAsync(modelId);
}
