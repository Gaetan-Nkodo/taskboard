using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        Log.Debug("DB: Fetching User {@UserQuery}", new { Id = id });

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        Log.Debug("DB: UserFetched {@User}", new { Found = user != null, Id = id });

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        Log.Debug("DB: Fetching UserByEmail {@UserQuery}", new { Email = email });

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

        Log.Debug("DB: UserFetchedByEmail {@User}", new { Found = user != null, Email = email });

        return user;
    }

    public async Task AddAsync(User user)
    {
        Log.Information("DB: UserInsert {@User}", new { user.Id, user.Email });

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}
