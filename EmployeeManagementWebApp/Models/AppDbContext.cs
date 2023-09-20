using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementWebApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
                
        }

        DbSet<Employee> Employees { get; set;}
    }
}
