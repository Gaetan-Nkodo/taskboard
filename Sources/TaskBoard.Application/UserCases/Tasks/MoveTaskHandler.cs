using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Tasks;

public class MoveTaskHandler
{
    private readonly ITaskRepository _repo;

    public MoveTaskHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(Guid taskId, Guid userId, MoveTaskRequest request)
    {
        var task = await _repo.GetByIdAsync(taskId, userId);
        if (task is null)
            throw new KeyNotFoundException("Task not found or not accessible.");

        var oldColumnId = task.ColumnId;
        var newColumnId = request.ColumnId;

        // Charger les tâches de l’ancienne colonne
        var oldColumnTasks = await _repo.GetByColumnAsync(oldColumnId, userId)
            ?? new List<TaskItem>();

        // Charger les tâches de la nouvelle colonne
        var newColumnTasks = await _repo.GetByColumnAsync(newColumnId, userId)
            ?? new List<TaskItem>();

        // Si même colonne → simple réorganisation
        if (oldColumnId == newColumnId)
        {
            oldColumnTasks = oldColumnTasks.Where(t => t.Id != taskId).ToList();

            var insertIndex = Math.Clamp(request.Order, 0, oldColumnTasks.Count);
            oldColumnTasks.Insert(insertIndex, task);

            for (int i = 0; i < oldColumnTasks.Count; i++)
                oldColumnTasks[i].MoveToColumn(oldColumnId, i);

            await _repo.SaveAllAsync(oldColumnTasks);
            return;
        }

        // Sinon → déplacement vers une autre colonne
        oldColumnTasks = oldColumnTasks.Where(t => t.Id != taskId).ToList();

        var newIndex = Math.Clamp(request.Order, 0, newColumnTasks.Count);
        newColumnTasks.Insert(newIndex, task);

        // Recalcul ancienne colonne
        for (int i = 0; i < oldColumnTasks.Count; i++)
            oldColumnTasks[i].MoveToColumn(oldColumnId, i);

        // Recalcul nouvelle colonne
        for (int i = 0; i < newColumnTasks.Count; i++)
            newColumnTasks[i].MoveToColumn(newColumnId, i);

        await _repo.SaveAllAsync(oldColumnTasks.Concat(newColumnTasks));
    }
}
