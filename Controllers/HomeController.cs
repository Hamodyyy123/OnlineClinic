using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineClinic.Models;

namespace OnlineClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Serve the raw cshtml file as static HTML (matching your current Index approach)
        public IActionResult Index()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "Index.cshtml");
            return PhysicalFile(filePath, "text/html");
        }

        public IActionResult About()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "About.cshtml");
            return PhysicalFile(filePath, "text/html");
        }

        public IActionResult Services()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "Services.cshtml");
            return PhysicalFile(filePath, "text/html");
        }

        public IActionResult Doctors()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "Doctors.cshtml");
            return PhysicalFile(filePath, "text/html");
        }

        public IActionResult Contact()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "Contact.cshtml");
            return PhysicalFile(filePath, "text/html");
        }

        public IActionResult Privacy()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "Privacy.cshtml");
            return PhysicalFile(filePath, "text/html");
        }

        // Debug: report whether each view file exists on disk
        [HttpGet]
        public IActionResult DebugViewFiles()
        {
            var cwd = Directory.GetCurrentDirectory();
            string[] files = new[]
            {
                Path.Combine(cwd, "Views", "Home", "Index.cshtml"),
                Path.Combine(cwd, "Views", "Home", "About.cshtml"),
                Path.Combine(cwd, "Views", "Home", "Services.cshtml"),
                Path.Combine(cwd, "Views", "Home", "Doctors.cshtml"),
                Path.Combine(cwd, "Views", "Home", "Contact.cshtml"),
                Path.Combine(cwd, "Views", "Home", "Privacy.cshtml")
            };

            var results = files.Select(f => new { Path = f, Exists = System.IO.File.Exists(f) });

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"CurrentDir: {cwd}");
            foreach (var r in results)
            {
                sb.AppendLine($"File: {r.Path}");
                sb.AppendLine($"Exists: {r.Exists}");
            }

            return Content(sb.ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}