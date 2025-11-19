using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Utilities;

public interface IEventFilter {
    IReadOnlyList<EnumEvenFilterOptions> AvailableFilters { get; }

    IQueryable<EventInfo> ApplyFilter(IQueryable<EventInfo> query, EventFilterOptions options); 
}
