using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DatabaseSchema.Model;

namespace DatabaseSchema.Services
{

        public interface IEmployeeService
        {
            Task AddEmployeeAsync(int id, string name, decimal salary, CancellationToken ct = default);
            Task<Employee?> GetEmployeeAsync(int id, CancellationToken ct = default);
        }
    
}
