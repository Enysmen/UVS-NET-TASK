# UVS Test
Build a console app which will add and get employees from a database using ORM (e.g. Entity Framework).

## Will test

 * Basic application architecture
 * Testability
 * Attention to detail

## Language

C#

## Requirements

 * Docker or a local sql instance
 * Dotnet core
 * powershell


// Proposed project structure:

// Solution: EmployeeApp.sln
// - src
//   - EmployeeApp.Console
//     - Program.cs
//     - Commands
//       - SetEmployeeCommand.cs
//       - GetEmployeeCommand.cs
//   - EmployeeApp.Core
//     - Models
//       - Employee.cs
//     - Services
//       - IEmployeeService.cs
//       - EmployeeService.cs
//   - EmployeeApp.Data
//     - EmployeeContext.cs
//     - Repositories
//       - IEmployeeRepository.cs
//       - EmployeeRepository.cs
// - tests
//   - EmployeeApp.Tests
//     - Services
//       - EmployeeServiceTests.cs
//     - Repositories
//       - EmployeeRepositoryTests.cs

// =======================
// EmployeeApp.Core/Models/Employee.cs
namespace EmployeeApp.Core.Models;

public class Employee
{
public int Id { get; set; }
public string Name { get; set; } = null!;
public decimal Salary { get; set; }
}

// =======================
// EmployeeApp.Data/EmployeeContext.cs
using EmployeeApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApp.Data;

public class EmployeeContext : DbContext
{
public EmployeeContext(DbContextOptions options)
: base(options) { }

public DbSet<Employee> Employees { get; set; } = null!;
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Employee>(eb =>
    {
        eb.ToTable("employees");
        eb.HasKey(e => e.Id);
        eb.Property(e => e.Name).HasColumnName("employeename").IsRequired().HasMaxLength(128);
        eb.Property(e => e.Salary).HasColumnName("employeesalary");
    });
}

}

// =======================
// EmployeeApp.Data/Repositories/IEmployeeRepository.cs
using EmployeeApp.Core.Models;

namespace EmployeeApp.Data.Repositories;

public interface IEmployeeRepository
{
Task AddAsync(Employee employee, CancellationToken ct = default);
Task<Employee?> GetAsync(int id, CancellationToken ct = default);
}

// =======================
// EmployeeApp.Data/Repositories/EmployeeRepository.cs
using EmployeeApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApp.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
private readonly EmployeeContext _context;
public EmployeeRepository(EmployeeContext context) => _context = context;

public async Task AddAsync(Employee employee, CancellationToken ct = default)
{
    await _context.Employees.AddAsync(employee, ct);
    await _context.SaveChangesAsync(ct);
}

public async Task<Employee?> GetAsync(int id, CancellationToken ct = default)
    => await _context.Employees.FindAsync(new object[] { id }, ct);

}

// =======================
// EmployeeApp.Core/Services/IEmployeeService.cs
using EmployeeApp.Core.Models;

namespace EmployeeApp.Core.Services;

public interface IEmployeeService
{
Task AddEmployeeAsync(int id, string name, decimal salary, CancellationToken ct = default);
Task<Employee?> GetEmployeeAsync(int id, CancellationToken ct = default);
}

// =======================
// EmployeeApp.Core/Services/EmployeeService.cs
using EmployeeApp.Core.Models;
using EmployeeApp.Data.Repositories;

namespace EmployeeApp.Core.Services;

public class EmployeeService : IEmployeeService
{
private readonly IEmployeeRepository _repo;
public EmployeeService(IEmployeeRepository repo) => _repo = repo;

public async Task AddEmployeeAsync(int id, string name, decimal salary, CancellationToken ct = default)
{
    var employee = new Employee { Id = id, Name = name, Salary = salary };
    await _repo.AddAsync(employee, ct);
}

public async Task<Employee?> GetEmployeeAsync(int id, CancellationToken ct = default)
    => await _repo.GetAsync(id, ct);

}

// =======================
// EmployeeApp.Console/Commands/SetEmployeeCommand.cs
using System.CommandLine;
using System.CommandLine.Invocation;
using EmployeeApp.Core.Services;

namespace EmployeeApp.Console.Commands;

public static class SetEmployeeCommand
{
public static Command Create(IEmployeeService svc)
{
var cmd = new Command("set-employee", "Add a new employee")
{
new Option("--employeeId", "Employee ID") { IsRequired = true },
new Option("--employeeName", "Employee name") { IsRequired = true },
new Option("--employeeSalary", "Employee salary") { IsRequired = true }
};

    cmd.Handler = CommandHandler.Create<int, string, decimal>(
        async (employeeId, employeeName, employeeSalary) =>
        {
            await svc.AddEmployeeAsync(employeeId, employeeName, employeeSalary);
            Console.WriteLine("Employee added.");
        });

    return cmd;
}

}

// =======================
// EmployeeApp.Console/Commands/GetEmployeeCommand.cs
using System.CommandLine;
using System.CommandLine.Invocation;
using EmployeeApp.Core.Services;

namespace EmployeeApp.Console.Commands;

public static class GetEmployeeCommand
{
public static Command Create(IEmployeeService svc)
{
var cmd = new Command("get-employee", "Retrieve employee by ID")
{
new Option("--employeeId", "Employee ID") { IsRequired = true }
};

    cmd.Handler = CommandHandler.Create<int>(
        async (employeeId) =>
        {
            var emp = await svc.GetEmployeeAsync(employeeId);
            if (emp is null)
                Console.WriteLine("Employee not found.");
            else
                Console.WriteLine($"Id={emp.Id}, Name={emp.Name}, Salary={emp.Salary}");
        });

    return cmd;
}

}

// =======================
// EmployeeApp.Console/Program.cs
using System.CommandLine;
using EmployeeApp.Console.Commands;
using EmployeeApp.Core.Services;
using EmployeeApp.Data;
using EmployeeApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
.ConfigureServices((context, services) =>
{
services.AddDbContext(opts =>
opts.UseNpgsql(context.Configuration.GetConnectionString("Default")));
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddScoped<IEmployeeService, EmployeeService>();
})
.Build();

var svc = host.Services.GetRequiredService();

// Root command
var root = new RootCommand("Employee Management CLI");
root.AddCommand(SetEmployeeCommand.Create(svc));
root.AddCommand(GetEmployeeCommand.Create(svc));

return await root.InvokeAsync(args);
