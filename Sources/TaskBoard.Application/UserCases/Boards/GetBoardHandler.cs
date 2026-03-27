using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Boards;

public class GetBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<object> Handle(Guid boardId)
    {
        var board = await _boardRepository.GetByIdAsync(boardId);

        if (board is null)
            throw new NotFoundException($"Board with ID {boardId} not found.");

        return new
        {
            board.Id,
            board.Name,
            board.Description,
            Tasks = board.Tasks.Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.Icon,
                t.Status
            })
        };
    }
}
