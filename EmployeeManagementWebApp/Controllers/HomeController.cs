using EmployeeManagementWebApp.Models;
using EmployeeManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace EmployeeManagementWebApp.Controllers
{
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
            IWebHostEnvironment webHostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            _webHostingEnvironment = webHostingEnvironment;
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
        public IActionResult Create(EmployeeCreateViewModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                // If the Photo property on the incoming model object is not null, then the user has selected an image to upload.
                if (employeeModel.Photo != null)
                {
                    // The image must be uploaded to the images folder in wwwroot. To get the path of the wwwroot folder we are using the inject
                    // WebHostingEnvironment service provided by ASP.NET Core

                    string uploadsFolder = Path.Combine(_webHostingEnvironment.WebRootPath, "images");

                    // To make sure the file name is unique we are appending a new GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeModel.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Use CopyTo() method provided by IFormFile interface to copy the file to wwwroot/images folder
                    employeeModel.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Employee employee = new Employee
                {
                    Name = employeeModel.Name,
                    Email = employeeModel.Email,
                    Department = employeeModel.Department,
                    // Use CopyTo() method provided by IFormFile interface to copy the file to wwwroot/images folder
                    PhotoPath = uniqueFileName
                };
                Employee newEmployee = _employeeRepository.Add(employee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
    }
}
