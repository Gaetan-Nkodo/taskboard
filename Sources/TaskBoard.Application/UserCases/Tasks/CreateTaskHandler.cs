using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

public class CreateTaskHandler
{
    private readonly IBoardRepository _boardRepository;
    private readonly ITaskRepository _taskRepository;

    public CreateTaskHandler(IBoardRepository boardRepository, ITaskRepository taskRepository)
    {
        _boardRepository = boardRepository;
        _taskRepository = taskRepository;
    }

    public async Task<Guid> Handle(Guid boardId, Guid userId, CreateTaskRequest request)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, userId);
        if (board is null)
            throw new Exception("Board not found or access denied.");

        var column = board.Columns.FirstOrDefault(c => c.Id == request.ColumnId);
        if (column is null)
            throw new Exception("Column not found.");

        // On crée la Task
        var task = new TaskItem(
            column.Id,
            request.Name,
            request.Description,
            request.Icon,
            order: column.Tasks.Count
        );

        // On l'ajoute directement dans la DB
        await _taskRepository.AddAsync(task);

        return task.Id;
    }
}
