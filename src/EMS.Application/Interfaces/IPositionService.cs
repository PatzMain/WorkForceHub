using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Application.DTOs.Position;

namespace EMS.Application.Interfaces
{
    public interface IPositionService
    {
        Task<PositionDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<PositionDto>> GetAllAsync();
        Task<PositionDto> CreateAsync(PositionCreateDto dto);
        Task UpdateAsync(PositionUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
