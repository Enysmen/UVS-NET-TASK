using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseSchema.Model;

using Microsoft.EntityFrameworkCore;

namespace DatabaseSchema.DataBaseContext
{
    class EmployeeContext : DbContext
    {
        public DbSet<Employee>? Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          => optionsBuilder.UseNpgsql("Host=localhost;Port=7777;Database=uvsproject;Username=postgres;Password=guest");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var emp = modelBuilder.Entity<Employee>();
            emp.ToTable("employees");  

           
            emp.Property(e => e.Id).HasColumnName("employeeid");
            emp.Property(e => e.Name).HasColumnName("employeename");
            emp.Property(e => e.Salary).HasColumnName("employeesalary");
        }
    }
}
