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

        public ViewResult Details(int id)
        {
            Employee model = _employeeRepository.GetEmployee(id);
            return View(model);

        }
        //public JsonResult Index()
        //{
        //    //return View();
        //    return Json(new {id=1, name = "Siddhant"});
        //}
    }
}
