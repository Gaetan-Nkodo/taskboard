using Serilog;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class DeleteTaskHandler
{
    private readonly IBoardRepository _boardRepository;

    public DeleteTaskHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task Handle(Guid taskId, Guid userId)
    {
        // Récupère le board qui contient la task
        var board = await _boardRepository.GetByTaskIdAsync(taskId, userId);

        if (board is null)
            throw new NotFoundException($"Task with ID {taskId} not found or access denied.");

        if (board.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this task.");

        // Trouver la colonne contenant la task
        var column = board.Columns.FirstOrDefault(c => c.Tasks.Any(t => t.Id == taskId));
        if (column is null)
            throw new NotFoundException("Column containing the task not found.");

        // Trouver la task
        var task = column.Tasks.First(t => t.Id == taskId);

        Log.Information("Deleting task {@Task}", new { task.Id, task.Name });

        // Supprimer la task de la colonne
        var tasksList = column.Tasks.ToList();
        tasksList.Remove(task);

        // Mise à jour du board (agrégat racine)
        await _boardRepository.UpdateAsync(board);
    }
}
