using TaskBoard.Application.Requests;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class UpdateTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task Handle(Guid taskId, UpdateTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            throw new NotFoundException($"Task with ID {taskId} not found.");

        task.Update(
            request.Name,
            request.Description,
            request.Icon,
            request.Status
        );

        await _taskRepository.UpdateAsync(task);
    }
}
