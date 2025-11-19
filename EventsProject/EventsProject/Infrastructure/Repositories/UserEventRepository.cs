using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Infrastructure.Repositories;

public class UserEventRepository : IUserEventRepository<UserEvent> {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;

    public UserEventRepository(EventsProjectContext context) { _context = context; }

    //------------------------METHODS------------------------
    public IQueryable<UserEvent> GetAll(bool tracking = false) 
        => tracking ? _context.UserEvents : _context.UserEvents.AsNoTracking();
    
    public IQueryable<UserEvent> GetEventsByUserId(int userId, bool tracking = false) {
        var eventsByUser = _context.UserEvents.Where(ue => ue.UserId == userId).Include(e => e.Event);
        return tracking ? eventsByUser : eventsByUser.AsNoTracking();
    }

    public IQueryable<UserEvent> GetUsersByEventId(int eventId, bool tracking = false) {
        var usersByEvent = _context.UserEvents.Where(ue => ue.EventId == eventId).Include(e => e.User);
        return tracking ? usersByEvent : usersByEvent.AsNoTracking();
    }

    public async Task<UserEvent?> GetByIdsAsync(int userId, int eventId)
        => await _context.UserEvents.FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

    public async Task<Result> InsertAsync(UserEvent userEvent) {
        try {
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return Result.Ok($"User with id ({userEvent.UserId}) has been enrolled in " +
                $"event with id ({userEvent.EventId}) succesfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error enrolling user on event", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(UserEvent userEvent) {
        try {
            _context.Entry(userEvent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Result.Ok($"{userEvent.UserEventId} has been updated succesfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error updating userEvent", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int userId, int eventId) {
        try {
            UserEvent? userEvent = await _context.UserEvents
                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

            if (userEvent == null)
                return Result.Fail($"User Event with ids (User: {userId} - Event: {eventId}) doesn't exist");

            _context.UserEvents.Remove(userEvent);
            await _context.SaveChangesAsync();
            return Result.Ok($"Event with ids (User: {userId} - Event: {eventId}) has been deleted sucessfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error deleting userEvent", ex.Message);
        }
    }
}

