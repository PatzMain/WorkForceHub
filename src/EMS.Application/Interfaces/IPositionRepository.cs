using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IPositionRepository : IRepository<Position>
    {
        Task<Position?> GetPositionWithDetailsAsync(int id);
        Task<IReadOnlyList<Position>> GetAllPositionsWithDetailsAsync();
    }
}
