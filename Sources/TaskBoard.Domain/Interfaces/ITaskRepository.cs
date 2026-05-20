using TaskBoard.Domain.Entities;

namespace TaskBoard.Domain.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
}
