using EventsProject.Domain.Common;

namespace EventsProject.Domain.Abstractions.Utilities;

public record MostEnrolledEvent(
    int EventId, string Title, DateTime Date, string City, int Capacity, int EnrolledCount
);
public record MostEnrolledCity(string CityName, int EventsCount);
public record MostEnrolledCategory(int CategoryId, string CategoryName, int EventsCount);

public interface IDataMetrics {
    //------------------------UserMetrics------------------------
    Task<int> EnrollmentsCountByUserAsync(int userId);
    Task<int> CreatedEventsCountByUserAsync(int userId);
    Task<int> PendingEventsCountByUserAsync(int userId);

    //------------------------EventMetrics------------------------
    Task<int> TotalEventsAsync();
    Task<int> EventsCountByDateAsync(DateTime date);
    Task<int> ActiveEventsCountAsync();
    
    //------------------------GlobalMetrics------------------------
    Task<int> TotalUsersAsync();
    Task<MostEnrolledEvent?> MostEnrolledEventAsync();
    Task<MostEnrolledCity?> MostEventsCityAsync();
    Task<MostEnrolledCategory?> MostEventsCategoryAsync();
    Task<int> EventsCountByCityAsync(string city);
    Task<int> EventsCountByCategoryAsync(int categoryId);
    Task<int> EventsCountByStateAsync(EnumEventState eventState);
}
