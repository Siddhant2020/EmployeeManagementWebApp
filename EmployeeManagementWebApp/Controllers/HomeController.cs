using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            //return View();
            return "Hello from MVC";
        }
    }
}
