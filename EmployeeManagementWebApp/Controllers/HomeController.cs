using EmployeeManagementWebApp.Models;
using EmployeeManagementWebApp.ViewModels;
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

        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        public ViewResult Details(int id = 1)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Emplyee Details"
            };
            return View(homeDetailsViewModel);
        }

    }
}
