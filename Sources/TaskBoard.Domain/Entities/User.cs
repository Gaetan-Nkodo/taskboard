using TaskBoard.Domain.Exceptions;

namespace TaskBoard.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    private readonly List<Board> _boards = new();
    public IReadOnlyCollection<Board> Boards => _boards;

    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
    }

    public void AddBoard(Board board)
    {
        if (board.UserId != Id)
            throw new DomainException("Board does not belong to this user.");

        _boards.Add(board);
    }
}
