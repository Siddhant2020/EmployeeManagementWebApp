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
            ViewData["Employee"] = model;
            ViewData["PageTitle"] = "Employee Details";
            return View();

            //return View(model);    // passing the model
            //return View("Test");    // passing the view name
            //return View("../Test/Update.cshtml");    // passing the view name with relative path
            //return View("~/MyViews/Test.cshtml");    // passing the view name with absolute path

        }
        //public JsonResult Index()
        //{
        //    //return View();
        //    return Json(new {id=1, name = "Siddhant"});
        //}
    }
}
