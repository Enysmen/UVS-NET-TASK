using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DatabaseSchema.Model;

namespace DatabaseSchema.Repositories
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee, CancellationToken ct = default);
        Task<Employee?> GetAsync(int id, CancellationToken ct = default);
    }
}
