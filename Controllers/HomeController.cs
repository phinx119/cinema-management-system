using Microsoft.AspNetCore.Mvc;
using PRN_ASG3.Models;
using System.Diagnostics;

namespace PRN_ASG3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // This method handles the POST request when the form is submitted.
            // Process form data, perform authentication, and redirect as needed.

            if (IsValidLogin(username, password))
            {
                HttpContext.Session.SetString("user", username);
                // Redirect to a success page.
                return RedirectToAction("Index");
            }
            else
            {
                // Display an error message or return to the login page.
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

            if (username == config["User:Username"] && password == config["User:Password"])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}