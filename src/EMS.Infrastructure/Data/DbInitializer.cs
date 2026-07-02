using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Identity;

namespace EMS.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Check if database exists/apply migrations
            await context.Database.EnsureCreatedAsync();

            // Seed Departments if empty
            if (!context.Departments.Any())
            {
                var departments = new List<Department>
                {
                    new Department { Name = "Technology", Code = "TECH" },
                    new Department { Name = "Human Resources", Code = "HR" },
                    new Department { Name = "Finance & Accounting", Code = "FIN" },
                    new Department { Name = "Sales & Marketing", Code = "SALES" }
                };
                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }

            // Seed Positions if empty
            if (!context.Positions.Any())
            {
                var positions = new List<Position>
                {
                    new Position { Title = "Software Engineer", Description = "Develops software solutions", BaseSalary = 4500, SalaryGrade = "6" },
                    new Position { Title = "Senior Architect", Description = "Designs systems architecture", BaseSalary = 8000, SalaryGrade = "9" },
                    new Position { Title = "HR Specialist", Description = "Manages recruitment and relations", BaseSalary = 3800, SalaryGrade = "5" },
                    new Position { Title = "Financial Analyst", Description = "Analyzes business finance records", BaseSalary = 4200, SalaryGrade = "6" },
                    new Position { Title = "Account Executive", Description = "Drives client sales and conversion", BaseSalary = 3500, SalaryGrade = "4" }
                };
                await context.Positions.AddRangeAsync(positions);
                await context.SaveChangesAsync();
            }

            // Seed Employees if empty
            if (!context.Employees.Any())
            {
                var techDept = await context.Departments.FirstAsync(d => d.Code == "TECH");
                var hrDept = await context.Departments.FirstAsync(d => d.Code == "HR");
                var finDept = await context.Departments.FirstAsync(d => d.Code == "FIN");

                var devPos = await context.Positions.FirstAsync(p => p.Title == "Software Engineer");
                var archPos = await context.Positions.FirstAsync(p => p.Title == "Senior Architect");
                var hrPos = await context.Positions.FirstAsync(p => p.Title == "HR Specialist");
                var finPos = await context.Positions.FirstAsync(p => p.Title == "Financial Analyst");

                var employees = new List<Employee>
                {
                    new Employee 
                    { 
                        FirstName = "Alice", 
                        LastName = "Smith", 
                        Email = "alice.smith@workforcehub.com", 
                        Phone = "555-0101", 
                        DateOfBirth = new DateTime(1990, 5, 12), 
                        Gender = Gender.Female,
                        Address = "123 Main St, Seattle, WA",
                        EmployeeNumber = "EMP-00101",
                        DateJoined = DateTime.Today.AddYears(-3),
                        Status = EmploymentStatus.Active,
                        DepartmentId = techDept.Id,
                        PositionId = archPos.Id
                    },
                    new Employee 
                    { 
                        FirstName = "Bob", 
                        LastName = "Jones", 
                        Email = "bob.jones@workforcehub.com", 
                        Phone = "555-0102", 
                        DateOfBirth = new DateTime(1994, 9, 20), 
                        Gender = Gender.Male,
                        Address = "456 Oak Ave, Tacoma, WA",
                        EmployeeNumber = "EMP-00102",
                        DateJoined = DateTime.Today.AddYears(-1),
                        Status = EmploymentStatus.Active,
                        DepartmentId = techDept.Id,
                        PositionId = devPos.Id
                    },
                    new Employee 
                    { 
                        FirstName = "Carol", 
                        LastName = "White", 
                        Email = "carol.white@workforcehub.com", 
                        Phone = "555-0103", 
                        DateOfBirth = new DateTime(1988, 11, 4), 
                        Gender = Gender.Female,
                        Address = "789 Pine Rd, Bellevue, WA",
                        EmployeeNumber = "EMP-00103",
                        DateJoined = DateTime.Today.AddYears(-2),
                        Status = EmploymentStatus.Active,
                        DepartmentId = hrDept.Id,
                        PositionId = hrPos.Id
                    },
                    new Employee 
                    { 
                        FirstName = "Daniel", 
                        LastName = "Green", 
                        Email = "daniel.green@workforcehub.com", 
                        Phone = "555-0104", 
                        DateOfBirth = new DateTime(1992, 3, 15), 
                        Gender = Gender.Male,
                        Address = "321 Cedar Blvd, Tacoma, WA",
                        EmployeeNumber = "EMP-00104",
                        DateJoined = DateTime.Today.AddMonths(-6),
                        Status = EmploymentStatus.Active,
                        DepartmentId = finDept.Id,
                        PositionId = finPos.Id
                    }
                };

                await context.Employees.AddRangeAsync(employees);
                await context.SaveChangesAsync();

                // Set managers after employees exist
                var alice = await context.Employees.FirstAsync(e => e.Email == "alice.smith@workforcehub.com");
                var carol = await context.Employees.FirstAsync(e => e.Email == "carol.white@workforcehub.com");
                var bob = await context.Employees.FirstAsync(e => e.Email == "bob.jones@workforcehub.com");
                var daniel = await context.Employees.FirstAsync(e => e.Email == "daniel.green@workforcehub.com");


                techDept.ManagerId = alice.Id;
                hrDept.ManagerId = carol.Id;
                await context.SaveChangesAsync();

                // Link User logins to Employees (optional step for test accounts)
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                var defaultAdmin = await userManager.FindByEmailAsync("admin@workforcehub.com");
                if (defaultAdmin != null)
                {
                    defaultAdmin.EmployeeId = alice.Id;
                    await userManager.UpdateAsync(defaultAdmin);
                    
                    alice.ApplicationUserId = defaultAdmin.Id;
                    await context.SaveChangesAsync();
                }

                // Seed Leave Requests if empty
                if (!context.LeaveRequests.Any())
                {
                    var leaves = new List<LeaveRequest>
                    {
                        new LeaveRequest
                        {
                            EmployeeId = alice.Id,
                            LeaveType = LeaveType.Vacation,
                            StartDate = DateTime.Today.AddDays(10),
                            EndDate = DateTime.Today.AddDays(15),
                            Status = LeaveStatus.Approved,
                            Reason = "Annual family trip",
                            ReviewedById = carol.Id,
                            ReviewNotes = "Approved, cover available",
                            DateReviewed = DateTime.UtcNow.AddDays(-2)
                        },
                        new LeaveRequest
                        {
                            EmployeeId = bob.Id,
                            LeaveType = LeaveType.Sick,
                            StartDate = DateTime.Today.AddDays(-5),
                            EndDate = DateTime.Today.AddDays(-3),
                            Status = LeaveStatus.Approved,
                            Reason = "Flu symptoms",
                            ReviewedById = alice.Id,
                            ReviewNotes = "Feel better",
                            DateReviewed = DateTime.UtcNow.AddDays(-6)
                        },
                        new LeaveRequest
                        {
                            EmployeeId = daniel.Id,
                            LeaveType = LeaveType.Emergency,
                            StartDate = DateTime.Today.AddDays(1),
                            EndDate = DateTime.Today.AddDays(2),
                            Status = LeaveStatus.Pending,
                            Reason = "Urgent family matter"
                        }
                    };
                    await context.LeaveRequests.AddRangeAsync(leaves);
                    await context.SaveChangesAsync();
                }

                // Seed Payroll Records if empty
                if (!context.PayrollRecords.Any())
                {
                    var payrolls = new List<PayrollRecord>
                    {
                        new PayrollRecord
                        {
                            EmployeeId = alice.Id,
                            PayPeriodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1),
                            PayPeriodEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1),
                            GrossPay = 8000,
                            Deductions = 1500,
                            NetPay = 6500,
                            Status = PayrollStatus.Paid,
                            PaymentDate = DateTime.UtcNow.AddDays(-5),
                            Notes = "Standard monthly pay run"
                        },
                        new PayrollRecord
                        {
                            EmployeeId = bob.Id,
                            PayPeriodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1),
                            PayPeriodEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1),
                            GrossPay = 4500,
                            Deductions = 800,
                            NetPay = 3700,
                            Status = PayrollStatus.Paid,
                            PaymentDate = DateTime.UtcNow.AddDays(-5),
                            Notes = "Standard monthly pay run"
                        },
                        new PayrollRecord
                        {
                            EmployeeId = carol.Id,
                            PayPeriodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                            PayPeriodEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1),
                            GrossPay = 3800,
                            Deductions = 600,
                            NetPay = 3200,
                            Status = PayrollStatus.Draft,
                            Notes = "Current month draft pay statement"
                        }
                    };
                    await context.PayrollRecords.AddRangeAsync(payrolls);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
