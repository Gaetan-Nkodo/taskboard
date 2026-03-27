using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class CreateTaskHandler
{
    private readonly IBoardRepository _boardRepository;
    private readonly ITaskRepository _taskRepository;

    public CreateTaskHandler(IBoardRepository boardRepository, ITaskRepository taskRepository)
    {
        _boardRepository = boardRepository;
        _taskRepository = taskRepository;
    }

    public async Task<Guid> Handle(Guid boardId, CreateTaskRequest request)
    {
        var board = await _boardRepository.GetByIdAsync(boardId);

        if (board is null)
            throw new NotFoundException($"Board with ID {boardId} not found.");

        var task = new TaskItem(boardId, request.Name, request.Description, request.Icon, request.Status);

        board.AddTask(task);

        await _taskRepository.AddAsync(task);

        return task.Id;
    }
}
