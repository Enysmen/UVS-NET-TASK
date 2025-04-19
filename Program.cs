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
                })
                .Build();

            
            var svc = host.Services.GetRequiredService<IEmployeeService>();

            
            var root = new RootCommand("UVS Test Console App");

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
                    Console.WriteLine("Employee not found.");
                else
                    Console.WriteLine($"Id={emp.Id}, Name={emp.Name}, Salary={emp.Salary}");
            });
            root.AddCommand(get);

            
            return await root.InvokeAsync(args);










            
        }
    }
}





//// 1. Создаём корневую команду
//var rootCommand = new RootCommand("UVS Test Console Application");

//// 2. Команда set-employee
//var setCmd = new Command("set-employee", "Add a new employee")
//{
//    new Option<int>("--employeeId", "Employee ID") { IsRequired = true },
//    new Option<string>("--employeeName", "Employee Name") { IsRequired = true },
//    new Option<decimal>("--employeeSalary", "Employee Salary") { IsRequired = true }
//};
//setCmd.Handler = CommandHandler.Create<int, string, decimal>(async (employeeId, employeeName, employeeSalary) =>
//{
//    await using var ctx = new EmployeeContext();
//    var emp = new Employee
//    {
//        Id = employeeId,
//        Name = employeeName,
//        Salary = employeeSalary
//    };
//    ctx.Employees!.Add(emp);
//    await ctx.SaveChangesAsync();
//    Console.WriteLine($"Id={emp.Id}, Name={emp.Name}, Salary={emp.Salary}");
//});
//rootCommand.AddCommand(setCmd);

//// 3. Команда get-employee
//var getCmd = new Command("get-employee", "Retrieve an employee by ID")
//{
//    new Option<int>("--employeeId", "Employee ID") { IsRequired = true }
//};
//getCmd.Handler = CommandHandler.Create<int>(async employeeId =>
//{
//    await using var ctx = new EmployeeContext();
//    var emp = await ctx.Employees!.FindAsync(employeeId);
//    if (emp is null)
//        Console.WriteLine("Employee not found");
//    else
//        Console.WriteLine($"Id={emp.Id}, Name={emp.Name}, Salary={emp.Salary}");
//});
//rootCommand.AddCommand(getCmd);




//// 4. Запускаем парсер
//return await rootCommand.InvokeAsync(args);
















//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Npgsql;

//namespace DatabaseSchema
//{
//    class Program
//    {
//        private static readonly string Password = Environment.GetEnvironmentVariable("UvsTaskPassword") 
//            ?? throw new InvalidOperationException("You must set the UvsTaskPassword environment variable");
//        private static readonly string Database = Environment.GetEnvironmentVariable("UvsTaskDatabase") 
//            ?? throw new InvalidOperationException("You must set the UvsTaskDatabase environment variable");
//        private static readonly string Port = Environment.GetEnvironmentVariable("UvsTaskPort") 
//            ?? throw new InvalidOperationException("You must set the UvsTaskPort environment variable");
//        private static readonly string SchemaLocation = Environment.GetEnvironmentVariable("UvsTaskSchemaLocation") 
//            ?? throw new InvalidOperationException("You must set the UvsTaskSchemaLocation environment variable");

//        static async Task Main(string[] args)
//        {
//            Console.WriteLine("Waiting for database to start");
//            await TestConnection();

//            Console.WriteLine("Adding new database");            
//            await CreateDatabase();

//            Console.WriteLine("Adding database schema");
//            await ImportSchema();
//        }

//        private static async Task TestConnection()
//        {
//            Exception? latestException = null;
//            var then = DateTime.UtcNow;
//            while (DateTime.UtcNow - then < TimeSpan.FromMinutes(0.2))
//            {
//                try
//                {
//                    using var cnxn = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port};");
//                    await cnxn.OpenAsync();
//                    Console.WriteLine("Connection attempt succeeded");

//                    return;
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("Connection attempt failed");
//                    latestException = e;
//                    await Task.Delay(1000);
//                }
//            }

//            throw new InvalidOperationException($"Could not connect to database", latestException);
//        }

//        private static async Task CreateDatabase()
//        {
//            using var cnxn = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port};");
//            await cnxn.OpenAsync();

//            var command = cnxn.CreateCommand();
//            command.CommandText = $"CREATE DATABASE {Database}";

//            await command.ExecuteNonQueryAsync();
//        }

//        private static async Task ImportSchema()
//        {
//            using var cnxn = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port}; Database={Database};");
//            await cnxn.OpenAsync();

//            var command = cnxn.CreateCommand();
//            command.CommandText = await File.ReadAllTextAsync(SchemaLocation);

//            await command.ExecuteNonQueryAsync();
//        }
//    }
//}
