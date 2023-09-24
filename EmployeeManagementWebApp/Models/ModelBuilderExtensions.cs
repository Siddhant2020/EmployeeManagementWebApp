using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementWebApp.Models
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                   new Employee
                   {
                       Id = 1,
                       Name = "Mark",
                       Department = Dept.IT,
                       Email = "mark@gmail.com"
                   }, new Employee
                   {
                       Id = 2,
                       Name = "John",
                       Department = Dept.HR,
                       Email = "john@gmail.com"
                   }
               );
        }
    }
}
