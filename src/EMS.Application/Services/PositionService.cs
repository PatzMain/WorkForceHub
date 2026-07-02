using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EMS.Application.Common;
using EMS.Application.DTOs.Position;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Application.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repo;
        private readonly IMapper _mapper;

        public PositionService(IPositionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PositionDto?> GetByIdAsync(int id)
        {
            var position = await _repo.GetPositionWithDetailsAsync(id);
            if (position == null)
                throw new NotFoundException(nameof(Position), id);

            return _mapper.Map<PositionDto>(position);
        }

        public async Task<IReadOnlyList<PositionDto>> GetAllAsync()
        {
            var positions = await _repo.GetAllPositionsWithDetailsAsync();
            return _mapper.Map<IReadOnlyList<PositionDto>>(positions);
        }

        public async Task<PositionDto> CreateAsync(PositionCreateDto dto)
        {
            var position = _mapper.Map<Position>(dto);
            await _repo.AddAsync(position);
            await _repo.SaveChangesAsync();
            return _mapper.Map<PositionDto>(position);
        }

        public async Task UpdateAsync(PositionUpdateDto dto)
        {
            var position = await _repo.GetByIdAsync(dto.Id);
            if (position == null)
                throw new NotFoundException(nameof(Position), dto.Id);

            _mapper.Map(dto, position);
            await _repo.UpdateAsync(position);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var position = await _repo.GetPositionWithDetailsAsync(id);
            if (position == null)
                throw new NotFoundException(nameof(Position), id);

            // Business rule: cannot delete if position has active employees
            if (position.Employees != null && position.Employees.Count > 0)
            {
                throw new System.Exception("Cannot delete a position that is currently assigned to employees.");
            }

            await _repo.DeleteAsync(position);
            await _repo.SaveChangesAsync();
        }
    }
}
