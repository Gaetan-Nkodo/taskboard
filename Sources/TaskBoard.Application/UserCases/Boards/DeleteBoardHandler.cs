using Serilog;

namespace TaskBoard.Application.UseCases.Boards;

public class DeleteBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public DeleteBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid boardId, Guid userId)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, userId);

        if (board is null)
            throw new NotFoundException($"Board with ID {boardId} not found.");

        if (board.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this board.");

        await _boardRepository.DeleteAsync(board);

        Log.Information("BoardDeleted {@Board}", new
        {
            board.Id,
            board.UserId,
            board.Name
        });
    }
}
