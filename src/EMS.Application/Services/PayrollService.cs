using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EMS.Application.Common;
using EMS.Application.DTOs.Payroll;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.Application.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _repo;
        private readonly IMapper _mapper;

        public PayrollService(IPayrollRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PayrollRecordDto?> GetByIdAsync(int id)
        {
            var record = await _repo.GetPayrollWithDetailsAsync(id);
            if (record == null)
                throw new NotFoundException(nameof(PayrollRecord), id);

            return _mapper.Map<PayrollRecordDto>(record);
        }

        public async Task<IReadOnlyList<PayrollRecordDto>> GetByEmployeeIdAsync(int employeeId)
        {
            var list = await _repo.GetPayrollByEmployeeIdAsync(employeeId);
            return _mapper.Map<IReadOnlyList<PayrollRecordDto>>(list);
        }

        public async Task<IReadOnlyList<PayrollRecordDto>> GetAllAsync()
        {
            var list = await _repo.GetAllPayrollWithDetailsAsync();
            return _mapper.Map<IReadOnlyList<PayrollRecordDto>>(list);
        }

        public async Task<PagedResult<PayrollRecordDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var list = await _repo.GetAllPayrollWithDetailsAsync();
            var dtos = _mapper.Map<List<PayrollRecordDto>>(list);
            
            var pagedItems = System.Linq.Enumerable.ToList(
                System.Linq.Enumerable.Take(
                    System.Linq.Enumerable.Skip(dtos, (pageNumber - 1) * pageSize),
                    pageSize
                )
            );
            
            return new PagedResult<PayrollRecordDto>(pagedItems, dtos.Count, pageNumber, pageSize);
        }

        public async Task<PagedResult<PayrollRecordDto>> GetPagedByEmployeeIdAsync(int employeeId, int pageNumber, int pageSize)
        {
            var list = await _repo.GetPayrollByEmployeeIdAsync(employeeId);
            var dtos = _mapper.Map<List<PayrollRecordDto>>(list);
            
            var pagedItems = System.Linq.Enumerable.ToList(
                System.Linq.Enumerable.Take(
                    System.Linq.Enumerable.Skip(dtos, (pageNumber - 1) * pageSize),
                    pageSize
                )
            );
            
            return new PagedResult<PayrollRecordDto>(pagedItems, dtos.Count, pageNumber, pageSize);
        }

        public async Task<PayrollRecordDto> CreateAsync(PayrollRecordCreateDto dto)
        {
            var record = _mapper.Map<PayrollRecord>(dto);
            record.NetPay = record.GrossPay - record.Deductions;
            record.Status = PayrollStatus.Draft;

            await _repo.AddAsync(record);
            await _repo.SaveChangesAsync();

            return _mapper.Map<PayrollRecordDto>(record);
        }

        public async Task UpdateAsync(PayrollRecordUpdateDto dto)
        {
            var record = await _repo.GetByIdAsync(dto.Id);
            if (record == null)
                throw new NotFoundException(nameof(PayrollRecord), dto.Id);

            if (record.Status != PayrollStatus.Draft)
            {
                throw new Exception("Only payroll records in Draft status can be edited.");
            }

            _mapper.Map(dto, record);
            record.NetPay = record.GrossPay - record.Deductions;

            await _repo.UpdateAsync(record);
            await _repo.SaveChangesAsync();
        }

        public async Task ProcessAsync(int id)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record == null)
                throw new NotFoundException(nameof(PayrollRecord), id);

            if (record.Status != PayrollStatus.Draft)
            {
                throw new Exception("Only payroll records in Draft status can be processed.");
            }

            record.Status = PayrollStatus.Processed;
            await _repo.UpdateAsync(record);
            await _repo.SaveChangesAsync();
        }

        public async Task PayAsync(int id)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record == null)
                throw new NotFoundException(nameof(PayrollRecord), id);

            if (record.Status != PayrollStatus.Processed)
            {
                throw new Exception("Only processed payroll records can be paid.");
            }

            record.Status = PayrollStatus.Paid;
            record.PaymentDate = DateTime.UtcNow;

            await _repo.UpdateAsync(record);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record == null)
                throw new NotFoundException(nameof(PayrollRecord), id);

            if (record.Status != PayrollStatus.Draft)
            {
                throw new Exception("Only payroll records in Draft status can be deleted.");
            }

            await _repo.DeleteAsync(record);
            await _repo.SaveChangesAsync();
        }
    }
}
