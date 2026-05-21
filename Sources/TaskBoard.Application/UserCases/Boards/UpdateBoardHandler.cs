using Serilog;
using TaskBoard.Application.Requests;

namespace TaskBoard.Application.UseCases.Boards;

public class UpdateBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public UpdateBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid boardId, Guid userId, UpdateBoardRequest request)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, userId);

        if (board is null)
            throw new NotFoundException($"Board with ID {boardId} not found.");

        if (board.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this board.");

        board.Update(request.Name, request.Description);

        await _boardRepository.UpdateAsync(board);

        Log.Information("BoardUpdated {@Board}", new
        {
            board.Id,
            board.UserId,
            board.Name,
            board.Description
        });
    }
}
