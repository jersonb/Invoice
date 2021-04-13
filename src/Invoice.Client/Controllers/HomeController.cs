using Invoice.Client.Data;
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
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationData _dbcontext;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, ApplicationData dbcontext)
        {
            _logger = logger;
            _env = env;
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Print([FromForm] InvoiceModel invoice)
        {
            if(invoice is null)
            {
                throw new ArgumentNullException(nameof(invoice));
            }

            _dbcontext.Invoices.Add(new InvoiceData { Invoice = invoice });
            _dbcontext.SaveChangesAsync();

            var path = _env.WebRootFileProvider.GetFileInfo("LOGO.jpg")?.PhysicalPath;

            return File(new PdfHandler(new Uri(path), invoice).PdfInMemoryStream.ToArray(), MediaTypeNames.Application.Pdf, $"{invoice.Client.Name}_{invoice.Number}.pdf", true);
        }

        public IActionResult Create()
        {
            return View();
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