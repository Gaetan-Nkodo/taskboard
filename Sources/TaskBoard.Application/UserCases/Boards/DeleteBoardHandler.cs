using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Boards;

public class DeleteBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public DeleteBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid boardId)
    {
        var board = await _boardRepository.GetByIdAsync(boardId);

        if (board is null)
            throw new NotFoundException($"Board with ID {boardId} not found.");

        await _boardRepository.DeleteAsync(board);
    }
}
