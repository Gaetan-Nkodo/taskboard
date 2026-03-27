using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Infrastructure.Persistence.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly AppDbContext _db;

    public BoardRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Board?> GetByIdAsync(Guid id)
    {
        return await _db.Boards.Include(b => b.Tasks).AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Board>> GetByUserIdAsync(Guid userId)
    {
        return await _db.Boards.Where(b => b.UserId == userId).Include(b => b.Tasks).AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Board board)
    {
        _db.Boards.Add(board);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Board board)
    {
        _db.Boards.Update(board);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Board board)
    {
        _db.Boards.Remove(board);
        await _db.SaveChangesAsync();
    }
}
