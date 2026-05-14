namespace TaskBoard.Domain.Entities;

public class Column
{
    private readonly List<TaskItem> _tasks = new();

    public Guid Id { get; private set; }
    public Guid BoardId { get; private set; }
    public string Name { get; private set; } = default!;
    public int Order { get; private set; }

    public IReadOnlyCollection<TaskItem> Tasks => _tasks;

    private Column() { }

    public Column(Guid boardId, string name, int order)
    {
        Id = Guid.NewGuid();
        BoardId = boardId;
        Name = name;
        Order = order;
    }

    public TaskItem AddTask(string name, string? description = null, string? icon = null, int? order = null)
    {
        var nextOrder = order ?? (_tasks.Count == 0 ? 1 : _tasks.Max(t => t.Order) + 1);
        var task = new TaskItem(Id, name, description, icon, nextOrder);
        _tasks.Add(task);
        return task;
    }

    public void Update(string name)
    {
        Name = name;
    }
}
