using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

public interface IFilterService {
    IReadOnlyList<EnumEvenFilterOptions> AvailableFilters { get; }

    public Task<List<EventInfo>> FilterEventsAsync(EventFilterOptions options);
    public Task<List<EventInfo>> FilterEventsWithCategoriesAsync(EventFilterOptions options);
}
