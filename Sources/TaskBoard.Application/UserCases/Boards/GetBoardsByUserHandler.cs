namespace TaskBoard.Application.UseCases.Boards;

public class GetBoardsByUserHandler
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardsByUserHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<List<object>> Handle(Guid userId)
    {
        var boards = await _boardRepository.GetByUserIdAsync(userId);

        return boards
            .Select(b => new { b.Id, b.Name, b.Description })
            .ToList<object>();
    }
}
