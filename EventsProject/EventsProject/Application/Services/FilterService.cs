using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Application.Services;

public class FilterService : IFilterService {
    //------------------------INITIALIZATION------------------------
    private readonly IRepository<EventInfo> _eventInfoRepository;
    private readonly IEventFilter _eventFilter;
    public IReadOnlyList<EnumEvenFilterOptions> AvailableFilters { get; }
    
    public FilterService(IRepository<EventInfo> eventInfoRepository, IEventFilter eventFilter) {
        _eventInfoRepository = eventInfoRepository;
        _eventFilter = eventFilter;
        AvailableFilters = _eventFilter.AvailableFilters;
    }

    //------------------------METHODS------------------------
    public async Task<List<EventInfo>> FilterEventsAsync(EventFilterOptions options)
        => await _eventFilter.ApplyFilter(_eventInfoRepository.GetAll(), options).ToListAsync();
    
    public async Task<List<EventInfo>> FilterEventsWithCategoriesAsync(EventFilterOptions options)
        => await _eventFilter.ApplyFilter(_eventInfoRepository.GetAll().Include(e => e.Cat), options).ToListAsync();
}
