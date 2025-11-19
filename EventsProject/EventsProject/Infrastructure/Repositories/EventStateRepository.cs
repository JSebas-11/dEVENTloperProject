using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Infrastructure.Repositories;

public class EventStateRepository : IReadOnlyRepository<EventState> {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;

    public EventStateRepository(EventsProjectContext context) { _context = context; }

    //------------------------METHODS------------------------
    public IQueryable<EventState> GetAll() 
        => _context.EventStates.AsNoTracking();
    public async Task<EventState?> GetByIdAsync(int modelId) 
        => await _context.EventStates.FindAsync(modelId);
}
