using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;

public class Board
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public string Name { get; private set; }
    public string? Description { get; private set; }

    private readonly List<TaskItem> _tasks = new();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks;

    public User? User { get; private set; }

    public Board(Guid userId, string name, string? description = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Description = description;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void AddTask(TaskItem task)
    {
        if (task.BoardId != Id)
            throw new DomainException("Task does not belong to this board.");

        _tasks.Add(task);
    }
}
