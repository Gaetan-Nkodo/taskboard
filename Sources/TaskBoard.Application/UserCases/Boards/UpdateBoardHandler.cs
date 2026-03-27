using TaskBoard.Application.Requests;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Boards;

public class UpdateBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public UpdateBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid boardId, UpdateBoardRequest request)
    {
        var board = await _boardRepository.GetByIdAsync(boardId);

        if (board is null)
            throw new NotFoundException($"Board with ID {boardId} not found.");

        board.Update(request.Name, request.Description);

        await _boardRepository.UpdateAsync(board);
    }
}
