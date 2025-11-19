using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventsProject.Infrastructure.Repositories;

public class UserAccountRepository : IRepository<UserAccount> {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;

    public UserAccountRepository(EventsProjectContext context) { _context = context; }

    //------------------------METHODS------------------------
    public IQueryable<UserAccount> GetAll(bool tracking = false) 
        => tracking ? _context.UserAccounts : _context.UserAccounts.AsNoTracking();
    public async Task<UserAccount?> GetByIdAsync(int modelId)
        => await _context.UserAccounts.FindAsync(modelId);

    public async Task<Result> InsertAsync(UserAccount user) {
        try {
            await _context.UserAccounts.AddAsync(user);
            await _context.SaveChangesAsync();
            return Result.Ok($"{user.UserName} has been added succesfully");
        } 
        catch (Exception ex) {
            return Result.Fail("There has been an error adding user", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(UserAccount model) {
        try {
            UserAccount? userTracked = await GetByIdAsync(model.UserId);

            if (userTracked == null)
                return Result.Fail($"{model.UserName} was not found in DB");

            //Copiar valores de objeto nuevo (del parametro) al que esta trackeado
            _context.Entry(userTracked).CurrentValues.SetValues(model);

            if (_context.Entry(userTracked).State == EntityState.Modified) {
                await _context.SaveChangesAsync();
                return Result.Ok($"{userTracked.UserName} has been updated succesfully");
            }
            
            return Result.Fail($"{userTracked.UserName} was not modified");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error updating user", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int userId) {
        try {
            UserAccount? user = await _context.UserAccounts.FindAsync(userId);
            if (user == null)
                return Result.Fail($"User with id ({userId}) does not exist");

            _context.UserAccounts.Remove(user);
            await _context.SaveChangesAsync();
            return Result.Ok($"User with id {userId} has been deleted sucessfully");
        }
        catch (Exception ex) {
            return Result.Fail("There has been an error deleting user", ex.Message);
        }
    }
}
