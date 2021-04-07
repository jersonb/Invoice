using Invoice.Client.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Mime;

namespace Invoice.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Print()
        {
            var path = _env.WebRootFileProvider.GetFileInfo("LOGO.jpg")?.PhysicalPath;

            return File(new PdfGenerator.GeneratePdf(new Uri(path)).PdfInMemoryStream.ToArray(), MediaTypeNames.Application.Pdf, "teste.pdf", true);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}