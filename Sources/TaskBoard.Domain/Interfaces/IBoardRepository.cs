using TaskBoard.Domain.Entities;

namespace TaskBoard.Domain.Interfaces;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id);
    Task<List<Board>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Board board);
    Task UpdateAsync(Board board);
    Task DeleteAsync(Board board);
}

