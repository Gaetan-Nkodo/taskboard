namespace TaskBoard.Domain.Entities;

public class Board
{
    private readonly List<Column> _columns = new();

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public IReadOnlyCollection<Column> Columns => _columns;

    private Board() { }

    public Board(Guid userId, string name, string? description = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Description = description;
    }

    public Column AddColumn(string name, int order)
    {
        var column = new Column(Id, name, order);
        _columns.Add(column);
        return column;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
