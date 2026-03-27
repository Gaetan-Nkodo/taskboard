using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class DeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task Handle(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            throw new NotFoundException($"Task with ID {taskId} not found.");

        await _taskRepository.DeleteAsync(task);
    }
}
