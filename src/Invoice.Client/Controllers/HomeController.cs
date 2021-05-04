using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationData _dbcontext;

        public HomeController(IWebHostEnvironment env, ApplicationData dbcontext)
        {
            _env = env;
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Print([FromForm] InvoiceModel invoice)
        {
            try
            {
                if(invoice is null)
                {
                    throw new ArgumentNullException(nameof(invoice));
                }

                if(ModelState.IsValid)
                {
                    var path = _env.WebRootFileProvider.GetFileInfo("LOGO.jpg")?.PhysicalPath;

                    return File(new PdfHandler(new Uri(path), invoice).PdfInMemoryStream.ToArray(), MediaTypeNames.Application.Pdf, $"{invoice.Client.Name}_{invoice.Number}.pdf", true);
                }

                return View(nameof(Create), invoice);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                var entity = _dbcontext.Invoices.Add(new InvoiceData { Invoice = invoice });
                await _dbcontext.SaveChangesAsync();
                entity.State = EntityState.Detached;
            }
        }

        public IActionResult Create()
        {
            var invoice = new InvoiceModel();
            invoice.AddProduct(new Product());
            return View(invoice);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([Bind("Products")] InvoiceModel invoice)
        {
            invoice.AddProduct(new Product());
            await Task.CompletedTask;
            return PartialView("Products", invoice);
        }
    }
}