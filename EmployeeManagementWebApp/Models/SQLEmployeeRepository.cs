using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementWebApp.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<SQLEmployeeRepository> _logger;

        public SQLEmployeeRepository(AppDbContext dbContext,
            ILogger<SQLEmployeeRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public Employee Add(Employee employee)
        {
            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            //Employee employee = dbContext.Employees.FirstOrDefault(emp => emp.Id == id);
            Employee employee = _dbContext.Employees.Find(id);
            if (employee != null)
            {
                _dbContext.Employees.Remove(employee);
                _dbContext.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _dbContext.Employees;
        }

        public Employee GetEmployee(int id)
        {
            //return _dbContext.Employees.FirstOrDefault(emp=>emp.Id == id);
            return _dbContext.Employees.Find(id);
        }

        public Employee Update(Employee employeeChanges)
        {
            //Employee employee = dbContext.Employees.FirstOrDefault(emp => emp.Id == employeeChanges.Id);
            var employee = _dbContext.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _dbContext.SaveChanges();
            return employee.Entity;
        }
    }
}
