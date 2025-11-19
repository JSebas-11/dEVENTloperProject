using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Application.Utilities;

public class DataMetrics : IDataMetrics {
    //------------------------INITIALIZATION------------------------
    private readonly IUserEventRepository<UserEvent> _userEventRepository;
    private readonly IRepository<EventInfo> _eventInfoRepository;
    private readonly IRepository<UserAccount> _userAccountRepository;
    private readonly IReadOnlyRepository<Category> _categoryRepository;
    public DataMetrics(IUserEventRepository<UserEvent> userEventRepository, 
        IRepository<EventInfo> eventInfoRepository,
        IRepository<UserAccount> userAccountRepository,
        IReadOnlyRepository<Category> categoryRepository) 
    {
        _userEventRepository = userEventRepository;
        _eventInfoRepository = eventInfoRepository;
        _userAccountRepository = userAccountRepository;
        _categoryRepository = categoryRepository;
    }

    //------------------------METHODS------------------------
    //------------------------User------------------------
    public async Task<int> EnrollmentsCountByUserAsync(int userId) {
        var events = await _userEventRepository.GetEventsByUserId(userId).ToListAsync();
        return events.DistinctBy(e => e.EventId).Count();
    }
    
    public async Task<int> CreatedEventsCountByUserAsync(int userId)
        =>  await _eventInfoRepository.GetAll()
                    .CountAsync(e => e.CreatedById == userId);

    public async Task<int> PendingEventsCountByUserAsync(int userId)
        => await _eventInfoRepository.GetAll()
                    .CountAsync(e => e.CreatedById == userId && e.EventStateId == (int)EnumEventState.Pending);

    //------------------------Event------------------------
    public async Task<int> TotalEventsAsync() => await _eventInfoRepository.GetAll().CountAsync();
    public async Task<int> EventsCountByDateAsync(DateTime date) 
        => await _eventInfoRepository.GetAll().CountAsync(e => e.EventDate == date);
    public async Task<int> ActiveEventsCountAsync()
        => await _eventInfoRepository.GetAll().CountAsync(e => e.EventStateId == (int)EnumEventState.Active);

    //------------------------Global------------------------
    public async Task<int> TotalUsersAsync() => await _userAccountRepository.GetAll().CountAsync();
    public async Task<MostEnrolledEvent?> MostEnrolledEventAsync() {
        var topEvent = await _userEventRepository.GetAll()
                        //Obtener el id y la suma de sus tickets inscritos
                        .GroupBy(ue => ue.EventId)
                        .Select(g => new { EventId = g.Key, TotalTickets = g.Sum(ue => ue.TicketsAmount) })
                        //Obtener record con mas inscripciones y retornarlo
                        .OrderByDescending(x => x.TotalTickets)
                        .FirstOrDefaultAsync();

        if (topEvent == null) return null;

        var ei = await _eventInfoRepository.GetByIdAsync(topEvent.EventId);

        return ei == null ? null : new MostEnrolledEvent(
            ei.EventId, ei.Title, ei.EventDate, ei.EventCity, ei.Capacity, topEvent.TotalTickets
        );
    }
    public async Task<MostEnrolledCity?> MostEventsCityAsync() {
        var result = await _eventInfoRepository.GetAll()
            //Agrupar y contar cantidad de eventos por ciudad
            .GroupBy(ei => ei.EventCity)
            .Select(g => new { City = g.Key, EventsCount = g.Count() } )
            //Obtener ciudad con mas cantidad de eventos
            .OrderByDescending(e => e.EventsCount)
            .FirstOrDefaultAsync();

        return result == null ? null : new MostEnrolledCity(result.City, result.EventsCount);
    }
    public async Task<MostEnrolledCategory?> MostEventsCategoryAsync() {
        var catResult = await _eventInfoRepository.GetAll()
            //Agrupar y contar cantidad de eventos por categoria
            .GroupBy(ei => ei.CatId)
            //Crear objeto anonimo temporal para guardar info de cantidad de eventos por cat
            .Select(c => new { CatId = c.Key, CatCount = c.Count() } )
            .OrderByDescending(cat => cat.CatCount)
            .FirstOrDefaultAsync();

        if (catResult == null) return null;

        var cat = await _categoryRepository.GetByIdAsync((int)catResult.CatId);

        return cat == null ? null : new MostEnrolledCategory(
            cat.CatId, cat.CatName, catResult.CatCount
        );
    }

    public async Task<int> EventsCountByCityAsync(string city)
        => await _eventInfoRepository.GetAll()
            .CountAsync(ei => ei.EventCity.Equals(city, StringComparison.CurrentCultureIgnoreCase));

    public async Task<int> EventsCountByCategoryAsync(int categoryId)
        => await _eventInfoRepository.GetAll()
            .CountAsync(ei => ei.CatId == categoryId);

    public async Task<int> EventsCountByStateAsync(EnumEventState eventState)
        => await _eventInfoRepository.GetAll()
            .CountAsync(ei => ei.EventStateId == (int)eventState);
}
