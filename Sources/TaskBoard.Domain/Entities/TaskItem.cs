namespace TaskBoard.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public Guid ColumnId { get; private set; }
    public Column Column { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public int Order { get; private set; }

    private TaskItem() { }

    public TaskItem(Guid columnId, string name, string? description, string? icon, int order)
    {
        Id = Guid.NewGuid();
        ColumnId = columnId;
        Name = name;
        Description = description;
        Icon = icon;
        Order = order;
    }

    public void Update(string name, string? description, string? icon)
    {
        Name = name;
        Description = description;
        Icon = icon;
    }

    public void MoveToColumn(Guid newColumnId, int newOrder)
    {
        ColumnId = newColumnId;
        Order = newOrder;
    }
}
