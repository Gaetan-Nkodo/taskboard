using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TaskItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId)
    {
        return await _db.Tasks
            .Include(t => t.Column)
            .ThenInclude(c => c.Board)
            .Where(t => t.Id == taskId && t.Column.Board.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TaskItem>> GetByBoardAsync(Guid boardId, Guid userId)
    {
        return await _db.Tasks
            .Include(t => t.Column)
            .Where(t => t.Column.BoardId == boardId && t.Column.Board.UserId == userId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetByColumnAsync(Guid columnId, Guid userId)
    {
        return await _db.Tasks
            .Include(t => t.Column)
            .ThenInclude(c => c.Board)
            .Where(t => t.ColumnId == columnId && t.Column.Board.UserId == userId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    public async Task SaveAllAsync(IEnumerable<TaskItem> tasks)
    {
        _db.Tasks.UpdateRange(tasks);
        await _db.SaveChangesAsync();
    }
}
