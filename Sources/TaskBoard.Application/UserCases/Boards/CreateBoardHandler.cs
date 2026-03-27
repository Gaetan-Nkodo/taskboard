using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

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

        await _boardRepository.AddAsync(board);

        return board.Id;
    }
}
