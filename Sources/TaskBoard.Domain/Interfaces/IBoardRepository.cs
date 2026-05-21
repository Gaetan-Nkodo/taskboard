using TaskBoard.Domain.Entities;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid boardId, Guid userId);
    Task<List<Board>> GetByUserIdAsync(Guid userId);
    Task<Board?> GetByTaskIdAsync(Guid taskId, Guid userId);

    Task AddAsync(Board board);
    Task UpdateAsync(Board board);
    Task DeleteAsync(Board board);
}
