using TaskBoard.Application.DTOs;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class GetTasksByBoardHandler
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksByBoardHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskDto>> Handle(Guid boardId, Guid userId)
    {
        var tasks = await _taskRepository.GetByBoardAsync(boardId, userId);

        return tasks
            .Select(t => new TaskDto(
                t.Id,
                t.ColumnId,
                t.Name,
                t.Description,
                t.Icon,
                t.Order
            ))
            .ToList();
    }
}
