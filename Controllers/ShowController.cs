using Microsoft.AspNetCore.Mvc;

namespace PRN_ASG3.Controllers
{
    public class Show : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
