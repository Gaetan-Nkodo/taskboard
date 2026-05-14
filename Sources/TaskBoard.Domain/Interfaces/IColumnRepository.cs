using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Domain.Interfaces;

public interface IColumnRepository
{
        Task<Column?> GetByIdAsync(Guid id);
        Task<List<Column>> GetByBoardIdAsync(Guid boardId);
        Task AddAsync(Column column);
        Task UpdateAsync(Column column);
        Task DeleteAsync(Column column);
}
