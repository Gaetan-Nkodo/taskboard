using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.UseCases.Boards;

public class CreateBoardHandler
{
    private readonly IBoardRepository _boardRepository;

    public CreateBoardHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<Guid> Handle(Guid userId, CreateBoardRequest request)
    {
        var board = new Board(userId, request.Name, request.Description);

        board.AddColumn("Backlog", 1);
        board.AddColumn("Ready", 2);
        board.AddColumn("In Progress", 3);
        board.AddColumn("Review", 4);
        board.AddColumn("Done", 5);

        await _boardRepository.AddAsync(board);
        return board.Id;
    }
}
