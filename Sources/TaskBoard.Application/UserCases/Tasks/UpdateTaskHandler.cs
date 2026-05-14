using TaskBoard.Application.Requests;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class UpdateTaskHandler
{
    private readonly IBoardRepository _boardRepository;

    public UpdateTaskHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid taskId, Guid userId, UpdateTaskRequest request)
    {
        var board = await _boardRepository.GetByTaskIdAsync(taskId, userId);
        if (board is null)
            throw new Exception("Task not found or access denied.");

        var currentColumn = board.Columns.First(c => c.Tasks.Any(t => t.Id == taskId));
        var task = currentColumn.Tasks.First(t => t.Id == taskId);

        if (request.ColumnId != currentColumn.Id)
        {
            var newColumn = board.Columns.FirstOrDefault(c => c.Id == request.ColumnId);
            if (newColumn is null)
                throw new Exception("Target column not found.");

            var newOrder = newColumn.Tasks.Any() ? newColumn.Tasks.Max(t => t.Order) + 1 : 1;

            task.MoveToColumn(newColumn.Id, newOrder);
        }

        task.Update(request.Name, request.Description, request.Icon);

        await _boardRepository.UpdateAsync(board);
    }
}
