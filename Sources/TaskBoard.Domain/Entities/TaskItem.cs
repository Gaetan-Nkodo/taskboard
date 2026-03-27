namespace TaskBoard.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public Guid BoardId { get; private set; }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Icon { get; private set; }
    public string Status { get; private set; }

    public TaskItem(Guid boardId, string name, string? description, string icon = "📝", string status = "En cours")
    {
        Id = Guid.NewGuid();
        BoardId = boardId;
        Name = name;
        Icon = icon;
        Status = status;
        if (description != null)
            Description = description;
    }

    public void Update(string name, string? description, string icon, string status)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Status = status;
    }
}
