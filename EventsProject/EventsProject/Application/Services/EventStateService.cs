using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Application.Services;

public class EventStateService : IEventStateService {
    //------------------------INITIALIZATION------------------------
    private readonly IReadOnlyRepository<EventState> _eventStateRepository;

    public EventStateService(IReadOnlyRepository<EventState> eventStateRepository) {
        _eventStateRepository = eventStateRepository;
    }

    //------------------------METHODS------------------------
    public async Task<List<EventState>> GetEventStatesAsync() => await _eventStateRepository.GetAll().ToListAsync();
}
