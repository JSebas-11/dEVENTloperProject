using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Infrastructure.Repositories;

public class EventInfoRepository : IRepository<EventInfo> {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;

    public EventInfoRepository(EventsProjectContext contex) { _context = contex; }

    //------------------------METHODS------------------------
    public IQueryable<EventInfo> GetAll(bool tracking = false) 
        => tracking ? _context.EventInfos : _context.EventInfos.AsNoTracking();
    public async Task<EventInfo?> GetByIdAsync(int modelId) 
        => await _context.EventInfos.FindAsync(modelId);
    public async Task<Result> InsertAsync(EventInfo eventInfo) {
        try {
            await _context.EventInfos.AddAsync(eventInfo);
            await _context.SaveChangesAsync();
            return Result.Ok($"{eventInfo.Title} has been added successfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error adding event", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(EventInfo eventInfo) {
        try {
            EventInfo? eventInfoTracked = await GetByIdAsync(eventInfo.EventId);
            if (eventInfoTracked == null)
                return Result.Fail($"Event id ({eventInfo.EventId}) was not found");

            //Copiar valores de objeto nuevo (del parametro) al que esta trackeado
            _context.Entry(eventInfoTracked).CurrentValues.SetValues(eventInfo);

            if (_context.Entry(eventInfoTracked).State == EntityState.Modified) {
                //Modificar solo en caso de que el objeto tenga cambios
                await _context.SaveChangesAsync();
                return Result.Ok($"{eventInfo.Title} has been updated successfully");
            }

            return Result.Fail($"Event (id: {eventInfo.EventId} - title {eventInfo.Title}) was not modified");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error updating event", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int eventId) {
        try {
            EventInfo? eventObj = await _context.EventInfos.FindAsync(eventId);

            if (eventObj == null)
                return Result.Fail($"Event with id {eventId} doesn't exist");

            _context.EventInfos.Remove(eventObj);
            await _context.SaveChangesAsync();
            return Result.Ok($"Event with id {eventId} has been deleted successfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error deleting event", ex.Message);
        }
    }

}
