using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementWebApp.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext dbContext;

        public SQLEmployeeRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Employee Add(Employee employee)
        {
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            //Employee employee = dbContext.Employees.FirstOrDefault(emp => emp.Id == id);
            Employee employee = dbContext.Employees.Find(id);
            if (employee != null)
            {
                dbContext.Employees.Remove(employee);
                dbContext.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return dbContext.Employees;
        }

        public Employee GetEmployee(int id)
        {
            return dbContext.Employees.FirstOrDefault(emp=>emp.Id == id);
        }

        public Employee Update(Employee employeeChanges)
        {
            //Employee employee = dbContext.Employees.FirstOrDefault(emp => emp.Id == employeeChanges.Id);
            var employee = dbContext.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dbContext.SaveChanges();
            return employee.Entity;
        }
    }
}
