﻿using System.Collections.Generic;

namespace EmployeeManagementWebApp.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int id);
        IEnumerable<Employee> GetAllEmployee();
    }
}
