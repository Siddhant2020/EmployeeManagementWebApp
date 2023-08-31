using EmployeeManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public string Index()
        {
            return _employeeRepository.GetEmployee(1).Name;
        }

        public ViewResult Details(int id = 1)
        {
            Employee model = _employeeRepository.GetEmployee(id);
            ViewBag.Employee = model;
            ViewBag.PageTitle = "Emplyee Details";
            return View();
        }

    }
}
