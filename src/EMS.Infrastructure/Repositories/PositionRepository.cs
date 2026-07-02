using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Application.Interfaces;
using EMS.Infrastructure.Data;

namespace EMS.Infrastructure.Repositories
{
    public class PositionRepository : Repository<Position>, IPositionRepository
    {
        public PositionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Position?> GetPositionWithDetailsAsync(int id)
        {
            return await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Position>> GetAllPositionsWithDetailsAsync()
        {
            return await _context.Positions
                .Include(p => p.Employees)
                .ToListAsync();
        }
    }
}
