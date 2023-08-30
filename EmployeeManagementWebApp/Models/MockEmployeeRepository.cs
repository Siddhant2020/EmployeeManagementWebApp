﻿using System.Collections.Generic;
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
                new Employee() {Id =  1, Name = "Mary", Department = "HR", Email = "mary@Zmail.com"},
                new Employee() {Id =  2, Name = "Brent", Department = "IT", Email = "brent@Zmail.com"},
                new Employee() {Id =  3, Name = "May", Department = "IT", Email = "may@Zmail.com"}
            };
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(emp => emp.Id == id);
        }
    }
}