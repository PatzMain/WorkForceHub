using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Domain.Common;
using EMS.Infrastructure.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure soft delete query filters
            builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
            builder.Entity<Position>().HasQueryFilter(p => !p.IsDeleted);

            // Department -> Employees relationship
            builder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department Manager cyclical relationship
            builder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Position -> Employees relationship
            builder.Entity<Position>()
                .HasMany(p => p.Employees)
                .WithOne(e => e.Position)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee -> LeaveRequests relationship
            builder.Entity<Employee>()
                .HasMany(e => e.LeaveRequests)
                .WithOne(l => l.Employee)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // LeaveRequest Reviewer cyclical relationship
            builder.Entity<LeaveRequest>()
                .HasOne(l => l.ReviewedBy)
                .WithMany()
                .HasForeignKey(l => l.ReviewedById)
                .OnDelete(DeleteBehavior.NoAction);

            // Employee -> PayrollRecords relationship
            builder.Entity<Employee>()
                .HasMany(e => e.PayrollRecords)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser -> Employee relationship (optional 1-to-1)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Employee)
                .WithMany()
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee's link to Identity User ID
            builder.Entity<Employee>()
                .Property(e => e.ApplicationUserId)
                .HasMaxLength(450); // Matches default Identity key size
        }

        public override int SaveChanges()
        {
            ApplyAuditAndSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditAndSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditAndSoftDelete()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                // Audit properties
                if (entry.Entity is BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                // Soft delete implementation
                if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    softDeleteEntity.IsDeleted = true;

                    // If it is also a BaseEntity, update the UpdatedAt property
                    if (entry.Entity is BaseEntity entity)
                    {
                        entity.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
