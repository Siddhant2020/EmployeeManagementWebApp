using EmployeeManagementWebApp.Models;
using EmployeeManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementWebApp.Controllers
{
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        //[Route("~/")]   //Map via Attribute Routing when url just contains the domain name http://localhost:38315/ or http://localhost:38315
        //[Route("")] //Map via Attribute Routing when url contains just the controller name http://localhost:38315/home or http://localhost:38315/home/
        //[Route("[action]")]    //Map via Attribute Routing when url contains the controller name and the action method as well http://localhost:38315/home/index
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        /// <summary>
        /// //Map via Attribute Routing when url contains controller,action and nullable id 
        /// for example : http://localhost:38315/home/details or http://localhost:38315/home/details/ or http://localhost:38315/home/details/1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("[action]/{id?}")]   
        public ViewResult Details(int? id = 1)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "Emplyee Details"
            };
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee newEmployee = _employeeRepository.Add(employee);
                //return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
    }
}
