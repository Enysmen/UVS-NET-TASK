using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSchema.DataBaseContext;

using DatabaseSchema.Model;

namespace DatabaseSchema.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _ctx;
        public EmployeeRepository(EmployeeContext ctx) => _ctx = ctx;

        public async Task AddAsync(Employee employee, CancellationToken ct = default)
        {
            await _ctx.Employees.AddAsync(employee, ct);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task<Employee?> GetAsync(int id, CancellationToken ct = default) =>
            await _ctx.Employees.FindAsync(new object[] { id }, ct);
    }
}
