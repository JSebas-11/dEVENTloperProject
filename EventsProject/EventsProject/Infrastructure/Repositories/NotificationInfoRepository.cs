using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Infrastructure.Repositories;

public class NotificationInfoRepository : IRepository<NotificationInfo> {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;

    public NotificationInfoRepository(EventsProjectContext contex) { _context = contex; }

    //------------------------METHODS------------------------
    public IQueryable<NotificationInfo> GetAll(bool tracking = false) 
        => tracking ? _context.NotificationInfos : _context.NotificationInfos.AsNoTracking();

    public async Task<NotificationInfo?> GetByIdAsync(int modelId) 
        => await _context.NotificationInfos.FindAsync(modelId);

    public async Task<Result> InsertAsync(NotificationInfo notification) {
        try {
            await _context.NotificationInfos.AddAsync(notification);
            await _context.SaveChangesAsync();
            return Result.Ok($"Notification with id ({notification.NotificationId}) has been added succesfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error adding notification", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(NotificationInfo notification) {
        try {
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Result.Ok($"Notification with id ({notification.NotificationId}) has been updated succesfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error updating notification", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int modelId) {
        try {
            NotificationInfo? notif = await _context.NotificationInfos.FindAsync(modelId);

            if (notif == null)
                return Result.Fail($"Notification with id {modelId} doesn't exist");

            _context.NotificationInfos.Remove(notif);
            await _context.SaveChangesAsync();
            return Result.Ok($"Notification with id {modelId} has been deleted sucessfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error deleting notification", ex.Message);
        }
    }
}
