﻿using EmployeeManagementWebApp.Models;
using EmployeeManagementWebApp.Security;
using EmployeeManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace EmployeeManagementWebApp.Controllers
{
    //[Route("[controller]")]
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProtector _protector;

        public HomeController(IEmployeeRepository employeeRepository,
            IWebHostEnvironment webHostingEnvironment,
            ILogger<HomeController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            _webHostingEnvironment = webHostingEnvironment;
            _logger = logger;
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRoutValue);
        }

        //[Route("~/")]   //Map via Attribute Routing when url just contains the domain name http://localhost:38315/ or http://localhost:38315
        //[Route("")] //Map via Attribute Routing when url contains just the controller name http://localhost:38315/home or http://localhost:38315/home/
        //[Route("[action]")]    //Map via Attribute Routing when url contains the controller name and the action method as well http://localhost:38315/home/index
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee().Select(e =>
            {
                e.EncryptedId = _protector.Protect(e.Id.ToString());
                return e;
            });
            return View(model);
        }

        /// <summary>
        /// //Map via Attribute Routing when url contains controller,action and nullable id 
        /// for example : http://localhost:38315/home/details or http://localhost:38315/home/details/ or http://localhost:38315/home/details/1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("[action]/{id?}")]   
        //public ViewResult Details(int? id)
        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            //throw new Exception();

            //_logger.LogTrace("LogTrace");
            //_logger.LogDebug("LogDebug");
            //_logger.LogInformation("LogInformation");
            //_logger.LogWarning("LogWarning");
            //_logger.LogError("LogError");
            //_logger.LogCritical("LogCritical");

            int? decryptedEmployeeId = Convert.ToInt32(_protector.Unprotect(id));

            Employee employee = _employeeRepository.GetEmployee(decryptedEmployeeId ?? 1);
            employee.EncryptedId = id;
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Emplyee Details"
            };
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        //[Authorize]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize]
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
                newEmployee.EncryptedId = _protector.Protect(employee.Id.ToString());
                return RedirectToAction("details", new { id = newEmployee.EncryptedId });
            }
            return View();
        }

        [HttpGet]
        //[Authorize]
        public ViewResult Edit(string id)
        {

            int decryptedEmployeeId = Convert.ToInt32(_protector.Unprotect(id));

            Employee employee = _employeeRepository.GetEmployee(decryptedEmployeeId);
            //Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        //Through model binding, the action method parameter EmployeeEditViewModel receives the posted edit form data
        [HttpPost]
        //[Authorize]
        public IActionResult Edit(EmployeeEditViewModel employeeEditModel)
        {
            //check if the provided data is valid, if not rerender the edit view so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                //Retrieve the employee being edited from the database
                Employee employee = _employeeRepository.GetEmployee(employeeEditModel.Id);
                //update the employee object with the data in the model object
                employee.Name = employeeEditModel.Name;
                employee.Email = employeeEditModel.Email;
                employee.Department = employeeEditModel.Department;

                //If the user wants to change the photo, a new photo will be uploaded and the Photo property on the model object receives 
                //the uploaded photo. If the Photo property is null, user did not upload a new photo and keeps his existing photo
                if (employeeEditModel.Photo != null)
                {
                    //If a new photo is uploaded, the existing photo must be deleted. So check if there is an existing photo and delete
                    if (employeeEditModel.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostingEnvironment.WebRootPath, "images", employeeEditModel.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    //Save the new Photo in wwwroot/images folder and update PhotoPath property of the employee object which will be eventually saved in the database
                    employee.PhotoPath = ProcessUploadFile(employeeEditModel);

                    //Call update method on the repository service passing it the employee object to update the data in the database table
                    Employee updatedEmployee = _employeeRepository.Update(employee);
                    return RedirectToAction("index");
                }
            }
            return View(employeeEditModel);
        }

        private string ProcessUploadFile(EmployeeEditViewModel employeeEditModel)
        {
            string uniqueFileName = null;
            // If the Photo property on the incoming model object is not null, then the user has selected an image to upload.
            if (employeeEditModel.Photo != null)
            {
                // The image must be uploaded to the images folder in wwwroot. To get the path of the wwwroot folder we are using the inject
                // WebHostingEnvironment service provided by ASP.NET Core

                string uploadsFolder = Path.Combine(_webHostingEnvironment.WebRootPath, "images");

                // To make sure the file name is unique we are appending a new GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeEditModel.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Use CopyTo() method provided by IFormFile interface to copy the file to wwwroot/images folder
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    employeeEditModel.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }
            else
            {
                if (!string.IsNullOrEmpty(employee.PhotoPath))
                    System.IO.File.Delete(Path.Combine(_webHostingEnvironment.WebRootPath, "images", employee.PhotoPath));

                _employeeRepository.Delete(id);
                return RedirectToAction("index", "home");
            }
        }
    }
}
