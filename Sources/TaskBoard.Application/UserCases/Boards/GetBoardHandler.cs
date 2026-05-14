using TaskBoard.Application.DTOs;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Boards;

public class GetBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardDto> Handle(Guid boardId, Guid userId)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, userId);
        if (board is null)
            throw new Exception("Board not found or access denied.");

        var columns = board.Columns
            .OrderBy(c => c.Order)
            .Select(c =>
                new ColumnDto(
                    c.Id,
                    c.Name,
                    c.Order,
                    c.Tasks
                        .OrderBy(t => t.Order)
                        .Select(t =>
                            new TaskDto(
                                t.Id,
                                t.Name,
                                t.Description,
                                t.Icon,
                                t.Order
                            )
                        )
                        .ToList()
                )
            )
            .ToList();

        return new BoardDto(
            board.Id,
            board.Name,
            board.Description,
            columns
        );
    }
}
