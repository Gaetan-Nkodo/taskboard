using TaskBoard.Application.Requests;
using TaskBoard.Domain.Interfaces;

public class CreateTaskHandler
{
    private readonly IBoardRepository _boardRepository;

    public CreateTaskHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<Guid> Handle(Guid boardId, Guid userId, CreateTaskRequest request)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, userId);
        if (board is null)
            throw new Exception("Board not found or access denied.");

        var column = board.Columns.FirstOrDefault(c => c.Id == request.ColumnId);
        if (column is null)
            throw new Exception("Column not found.");

        var task = column.AddTask(request.Name, request.Description, request.Icon);

        await _boardRepository.UpdateAsync(board);
        return task.Id;
    }
}