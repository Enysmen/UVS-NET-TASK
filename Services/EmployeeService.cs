using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSchema.Model;

using DatabaseSchema.Repositories;

namespace DatabaseSchema.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        public EmployeeService(IEmployeeRepository repo) => _repo = repo;

        public Task AddEmployeeAsync(int id, string name, decimal salary, CancellationToken ct = default) =>
            _repo.AddAsync(new Employee { Id = id, Name = name, Salary = salary }, ct);

        public Task<Employee?> GetEmployeeAsync(int id, CancellationToken ct = default) =>
            _repo.GetAsync(id, ct);
    }
}
