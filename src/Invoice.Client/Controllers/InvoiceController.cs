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

        public async Task<IActionResult> Index()
        {
            var viewModel = await _dbcontext.Invoices
                                            .AsNoTracking()
                                            .Where(x => x.Invoice.IsActive)
                                            .Select(x => new InvoiceIndexViewModel
                                            {
                                                FindId = x.Invoice.FindId,
                                                Number = x.Invoice.Number,
                                                Date = x.Invoice.Date,
                                                Customer = $"{x.Invoice.Client.Name} - {x.Invoice.Client.LegalNumber}",
                                                Total = x.Invoice.Total
                                            })
                                            .ToListAsync();
            return View(viewModel);
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
                    await Save(invoice);
                }
            }
        }

        private async Task Save(InvoiceModel invoice)
        {
            invoice.CreateId();
            var entity = _dbcontext.Invoices.Add(new InvoiceData { Invoice = invoice });
            await _dbcontext.SaveChangesAsync();
            entity.State = EntityState.Detached;
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
            model.Products.Add(new Product { Quantity = 1, Value = "0,00" });

            var customers = await _dbcontext.Customers
                        .AsNoTracking()
                        .Where(x => x.Customer.IsActive)
                        .Select(x => new SelectListItem { Text = x.Customer.Name, Value = x.Customer.FindId })
                        .ToListAsync();

            ViewBag.Items = customers;

            return View(model);
        }

        
        public IActionResult AddProduct([Bind("Products")] InvoiceViewModel invoice)
        {
            invoice.Products.Add(new Product { Quantity = 1, Value = "0,00" });
            return PartialView("Products", invoice);
        }
    }
}