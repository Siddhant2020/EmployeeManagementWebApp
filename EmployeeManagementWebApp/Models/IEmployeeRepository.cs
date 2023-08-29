namespace EmployeeManagementWebApp.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int id);
    }
}
