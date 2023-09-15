using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementWebApp.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() {Id =  1, Name = "Mary", Department = Dept.HR, Email = "mary@Zmail.com"},
                new Employee() {Id =  2, Name = "Brent", Department = Dept.IT, Email = "brent@Zmail.com"},
                new Employee() {Id =  3, Name = "May", Department = Dept.Payroll, Email = "may@Zmail.com"}
            };
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(emp => emp.Id == id);
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _employeeList.Max(emp => emp.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }
    }
}
