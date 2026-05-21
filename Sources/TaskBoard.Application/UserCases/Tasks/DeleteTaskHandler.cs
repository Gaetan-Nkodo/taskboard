using Serilog;

namespace TaskBoard.Application.UseCases.Tasks;

public class DeleteTaskHandler
{
    private readonly IBoardRepository _boardRepository;

    public DeleteTaskHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid taskId, Guid userId)
    {
        var board = await _boardRepository.GetByTaskIdAsync(taskId, userId);

        if (board is null)
            throw new NotFoundException($"Task with ID {taskId} not found or access denied.");

        if (board.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this task.");

        var column = board.Columns.FirstOrDefault(c => c.Tasks.Any(t => t.Id == taskId));
        if (column is null)
            throw new NotFoundException("Column containing the task not found.");

        Log.Information("Deleting task {TaskId}", taskId);

        column.RemoveTask(taskId);

        await _boardRepository.UpdateAsync(board);
    }
}
