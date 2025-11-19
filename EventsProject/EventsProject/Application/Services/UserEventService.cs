using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Application.Services;

public class UserEventService : IUserEventService {
    //------------------------INITIALIZATION------------------------
    private readonly IUserEventRepository<UserEvent> _userEventRepository;
    private readonly IEventService _eventService;
    private readonly IRepository<EventInfo> _eventRepository;
    public UserEventService(IUserEventRepository<UserEvent> userEventRepository, IEventService eventService, IRepository<EventInfo> eventRepository) {
        _userEventRepository = userEventRepository;
        _eventService = eventService;
        _eventRepository = eventRepository;
    }

    //------------------------METHODS------------------------
    //------------------------innerMeths------------------------
    private async Task<List<EventInfo>> GetEventsUniqueByUser(int userId, bool tracking = false) {
        var events = await GetEventsByUserAsync(userId, tracking);
        return [.. events.DistinctBy(e => e.EventId)];
    }

    //------------------------getMeths------------------------
    public async Task<List<EventInfo>> GetEventsByUserAsync(int userId, bool tracking = false)
        => await _userEventRepository.GetEventsByUserId(userId, tracking).Select(ue => ue.Event).ToListAsync();

    public async Task<List<EventInfo>> GetUniqueEnrolledEventsByUserAsync(int userId, bool tracking = false) 
        => await GetEventsUniqueByUser(userId);

    public async Task<List<EventInfo>> GetLastNEventByUserAsync(int userId, int NumEvents = 5) {
        List<EventInfo> eventsByUser = await GetEventsUniqueByUser(userId, false);
        return [.. eventsByUser.Where(ei => ei.EventDate < DateTime.Today)
                .OrderByDescending(ei => ei.EventDate).Take(NumEvents)];
    }

    public async Task<List<EventInfo>> GetNextNEventByUserAsync(int userId, int NumEvents = 5) {
        List<EventInfo> eventsByUser = await GetEventsUniqueByUser(userId, false);
        return eventsByUser.Where(ei => ei.EventDate >= DateTime.Today)
                .OrderBy(ei => ei.EventDate).Take(NumEvents).ToList();
    }

    //------------------------enrrollMeths------------------------
    public int EnrrolledInEventCount(int eventId)
        => _userEventRepository.GetAll().Where(ue => ue.EventId == eventId)
                .Sum(ue => ue.TicketsAmount);

    public async Task<int> TicketsBoughtByUserInAsync(int userId, int eventId)
        => await _userEventRepository.GetAll()
                .Where(ue => ue.UserId == userId && ue.EventId == eventId)
                .SumAsync(ue => ue.TicketsAmount);
    
    public async Task<Result> EnrrollUserInAsync(int userId, int eventId, int amount) {
        //Validacion que evento efectivamente exista
        EventInfo? evennt = await _eventRepository.GetByIdAsync(eventId);
        if (evennt is null)
            return Result.Fail($"Event with id {eventId} does not exist");

        //Validacion de usuario no sobrepase limite de tickets
        int boughtTickets = await TicketsBoughtByUserInAsync(userId, eventId);
        if (boughtTickets + amount > ValidationConsts.MaxEventsByUser)
            return Result.Fail($"You can buy up to {ValidationConsts.MaxEventsByUser} tickets per user. You already have {boughtTickets} tickets");

        //Validacion de disponibilidad de tiquetes solicitados
        int enrolledTickets = EnrrolledInEventCount(eventId);
        int availableTickets = evennt.Capacity - enrolledTickets;      
        if (amount > availableTickets)
            return Result.Fail($"There is not enough tickets (available tickets: {availableTickets})");

        Result insertResult = await _userEventRepository.InsertAsync(new UserEvent(userId, eventId, amount));

        //Verificar y actualizar estado del evento si corresponde
        if (insertResult.Success)
            if (amount + enrolledTickets == evennt.Capacity) {
                Result result = await _eventService.ChangeEventStateAsync(evennt, (int)EnumEventState.SoldOut);
                Console.WriteLine(result.Description);
            }

        return insertResult;
    }
}
