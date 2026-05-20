using Microsoft.EntityFrameworkCore;
using Serilog;
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

    public async Task<List<Board>> GetByUserIdAsync(Guid userId)
    {
        Log.Debug("DB: Fetching BoardsByUser {@Query}", new { userId });

        var boards = await _db.Boards
            .Where(b => b.UserId == userId)
            .Include(b => b.Columns)
                .ThenInclude(c => c.Tasks)
            .ToListAsync();

        Log.Debug("DB: BoardsFetched {@Result}", new { Count = boards.Count, userId });

        return boards;
    }

    public async Task<Board?> GetByIdAsync(Guid boardId, Guid userId)
    {
        Log.Debug("DB: Fetching Board {@Query}", new { boardId, userId });

        return await _db.Boards
            .Where(b => b.Id == boardId && b.UserId == userId)
            .Include(b => b.Columns)
                .ThenInclude(c => c.Tasks)
            .FirstOrDefaultAsync();

    }

    public async Task<Board?> GetByTaskIdAsync(Guid taskId, Guid userId)
    {
        Log.Debug("DB: Fetching BoardByTask {@Query}", new { taskId, userId });

        return await _db.Boards
            .Where(b => b.UserId == userId)
            .Include(b => b.Columns)
                .ThenInclude(c => c.Tasks)
            .FirstOrDefaultAsync(b => b.Columns.Any(c => c.Tasks.Any(t => t.Id == taskId)));
    }

    public async Task AddAsync(Board board)
    {
        Log.Information("DB: BoardInsert {@Board}", new { board.Id, board.UserId, board.Name });

        _db.Boards.Add(board);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Board board)
    {
        Log.Information("DB: BoardUpdate {@Board}", new { board.Id, board.UserId, board.Name });

        _db.Boards.Update(board);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Board board)
    {
        Log.Warning("DB: BoardDelete {@Board}", new { board.Id, board.UserId, board.Name });

        _db.Boards.Remove(board);
        await _db.SaveChangesAsync();
    }
}
