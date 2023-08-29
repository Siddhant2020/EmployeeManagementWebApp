using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult Index()
        {
            //return View();
            return Json(new {id=1, name = "Siddhant"});
        }
    }
}
