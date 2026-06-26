using TaskBoard.Domain.Entities;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);

    Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId);

    Task<List<TaskItem>> GetByBoardAsync(Guid boardId, Guid userId);

    Task<List<TaskItem>> GetByColumnAsync(Guid columnId, Guid userId);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(TaskItem task);

    Task SaveAllAsync(IEnumerable<TaskItem> tasks);
}
