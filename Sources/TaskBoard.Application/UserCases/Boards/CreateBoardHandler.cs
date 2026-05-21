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

        // Colonnes par défaut
        var inProgress = board.AddColumn("In Progress", 1);
        var completed = board.AddColumn("Completed", 2);
        var wontDo = board.AddColumn("Won't Do", 3);

        // Tasks par défaut
        inProgress.AddTask("Task in Progress");
        completed.AddTask("Task Completed");
        wontDo.AddTask("Task Won't Do");

        await _boardRepository.AddAsync(board);
        return board.Id;
    }
}
