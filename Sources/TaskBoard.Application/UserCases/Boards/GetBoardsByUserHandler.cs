using TaskBoard.Application.DTOs;

namespace TaskBoard.Application.UseCases.Boards;

public class GetBoardsByUserHandler
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardsByUserHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<List<BoardDto>> Handle(Guid userId)
    {
        var boards = await _boardRepository.GetByUserIdAsync(userId);

        return boards
            .Select(b => new BoardDto(
                b.Id,
                b.Name,
                b.Description,
                b.Columns
                    .OrderBy(c => c.Order)
                    .Select(c => new ColumnDto(
                        c.Id,
                        c.Name,
                        c.Order,
                        c.Tasks
                            .OrderBy(t => t.Order)
                            .Select(t => new TaskDto(
                                t.Id,
                                t.ColumnId,
                                t.Name,
                                t.Description,
                                t.Icon,
                                t.Order
                            ))
                            .ToList()
                    ))
                    .ToList()
            ))
            .ToList();
    }
}
