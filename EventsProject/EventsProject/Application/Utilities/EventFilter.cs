using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Application.Utilities;

public class EventFilter : IEventFilter {
    public IReadOnlyList<EnumEvenFilterOptions> AvailableFilters => [
        EnumEvenFilterOptions.All, EnumEvenFilterOptions.Title, EnumEvenFilterOptions.City, EnumEvenFilterOptions.Place,
        EnumEvenFilterOptions.Date, EnumEvenFilterOptions.State, EnumEvenFilterOptions.Category
    ];

    public IQueryable<EventInfo> ApplyFilter(IQueryable<EventInfo> query, EventFilterOptions options) {
        string filterValue = options.FilterValue;
        switch (options.FilterField) {

            //No Filtro
            case EnumEvenFilterOptions.All:
                return query;

            //Filtro por titulo
            case EnumEvenFilterOptions.Title:
                query = query.Where(ei => ei.Title.ToUpper().Contains(filterValue.ToUpper()));
                break;

            //Filtro por lugar (DB almacena en UPPERCASE)
            case EnumEvenFilterOptions.Place:
                query = query.Where(ei => ei.EventPlace.Contains(filterValue.ToUpper()));
                break;

            //Filtro por ciudad (DB almacena en UPPERCASE)
            case EnumEvenFilterOptions.City:
                query = query.Where(ei => ei.EventCity.Contains(filterValue.ToUpper()));
                break;

            //Filtro por fecha (inicial o final)
            case EnumEvenFilterOptions.Date:
                if (DateTime.TryParse(filterValue, out DateTime date)) {
                    var nextDay = date.AddDays(1);
                    query = query.Where(ei => ei.InitialTime >= date && ei.InitialTime < nextDay
                                        || ei.EndTime >= date && ei.EndTime < nextDay);    
                }
                else { query = query.Where(ei => false); }
                break;

            //Filtro por estado
            case EnumEvenFilterOptions.State:
                if (int.TryParse(filterValue, out int stateId))
                    query = query.Where(ei => ei.EventStateId == stateId);
                 else
                    query = query.Where(ei => false);
                
                break;

            //Filtro por categoria
            case EnumEvenFilterOptions.Category:
                if (int.TryParse(filterValue, out int catId))
                    query = query.Where(ei => ei.CatId == catId);
                else
                    query = query.Where(ei => false);
                
                break;
        }
        return query;
    }
}
