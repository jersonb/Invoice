using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationData _dbcontext;

        public InvoiceController(IWebHostEnvironment env, ApplicationData dbcontext)
        {
            _env = env;
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Print([FromForm] InvoiceViewModel viewModel)
        {
            InvoiceModel invoice = null;
            try
            {
                if(ModelState.IsValid)
                {
                    var customer = await _dbcontext.Customers
                                   .SingleOrDefaultAsync(x => x.Customer.FindId == viewModel.SelectedItem);

                    var json = JsonConvert.SerializeObject(viewModel);
                    invoice = JsonConvert.DeserializeObject<InvoiceModel>(json);

                    invoice.Client = customer.Customer;

                    return Print(invoice);
                }

                return View(nameof(Create), viewModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(invoice != null)
                {
                    var entity = _dbcontext.Invoices.Add(new InvoiceData { Invoice = invoice });
                    await _dbcontext.SaveChangesAsync();
                    entity.State = EntityState.Detached;
                }
            }
        }

        private IActionResult Print(InvoiceModel invoice)
        {
            if(invoice is null)
            {
                throw new InvalidOperationException("Fatura em branco! Não pode ser gerada!");
            }

            var path = _env.WebRootFileProvider.GetFileInfo("LOGO.jpg")?.PhysicalPath;

            return File(new PdfHandler(new Uri(path), invoice).PdfInMemoryStream.ToArray(), MediaTypeNames.Application.Pdf, $"{invoice.Client.Name}_{invoice.Number}.pdf", true);
        }

        public async Task<IActionResult> Create()
        {
            var model = new InvoiceViewModel();

            var customers = await _dbcontext.Customers
                        .AsNoTracking()
                        .Where(x => x.Customer.IsActive)
                        .Select(x => new SelectListItem { Text = x.Customer.Name, Value = x.Customer.FindId })
                        .ToListAsync();

            ViewBag.Items = customers;

            return View(model);
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