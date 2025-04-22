using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using System.CommandLine.IO;
using System.IO;
using System.Threading;
using Npgsql;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DatabaseSchema.DataBaseContext;
using DatabaseSchema.Model;
using DatabaseSchema.Services;
using Microsoft.Extensions.Configuration;
using DatabaseSchema.Repositories;
using DatabaseSchema.Infrastructure;


namespace DatabaseSchema
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {

            using var host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    
                    services.AddDbContext<EmployeeContext>(opts =>
                        opts.UseNpgsql(ctx.Configuration.GetConnectionString("Default")!));
                    
                    services.AddScoped<IEmployeeRepository, EmployeeRepository>();
                    services.AddScoped<IEmployeeService, EmployeeService>();
                    services.AddScoped<DatabaseInitializer>();

                }).Build();

            
            var svc = host.Services.GetRequiredService<IEmployeeService>();
            var initializer = host.Services.GetRequiredService<DatabaseInitializer>();

            var root = new RootCommand("UVS Test Console App");
            

            var initDbCmd = new Command("init-db", "Initialize Postgres database and apply schema");
            initDbCmd.Handler = CommandHandler.Create(async () =>
            {
                await initializer.InitializeAsync();
                Console.WriteLine("Database initialized successfully.");
            });
            root.AddCommand(initDbCmd);

            var set = new Command("set-employee", "Add new employee");
            set.AddOption(new Option<int>("--employeeId", "Employee ID") { IsRequired = true });
            set.AddOption(new Option<string>("--employeeName", "Employee Name") { IsRequired = true });
            set.AddOption(new Option<decimal>("--employeeSalary", "Employee Salary") { IsRequired = true });
            set.Handler = CommandHandler.Create<int, string, decimal>(async (employeeId, employeeName, employeeSalary) =>
            {
                await svc.AddEmployeeAsync(employeeId, employeeName, employeeSalary);
                Console.WriteLine("Employee added.");
            });
            root.AddCommand(set);

            var get = new Command("get-employee", "Get employee by ID");
            get.AddOption(new Option<int>("--employeeId", "Employee ID") { IsRequired = true });
            get.Handler = CommandHandler.Create<int>(async employeeId =>
            {
                var emp = await svc.GetEmployeeAsync(employeeId);
                if (emp is null)
                {
                    Console.WriteLine("Employee not found.");
                }   
                else
                {
                    Console.WriteLine($"Id={emp.Id}, Name={emp.Name}, Salary={emp.Salary}");
                }  
            });
            root.AddCommand(get);   
            return await root.InvokeAsync(args);
        }
    }
}