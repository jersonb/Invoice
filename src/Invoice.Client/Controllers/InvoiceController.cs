using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationData _dbcontext;
        private EntityEntry<InvoiceData> _entity;

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
                                            .OrderByDescending(x => x.Invoice.Updated)
                                            .ThenBy(x => x.Invoice.Number)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Print([FromForm] InvoiceViewModel viewModel)
        {
            InvoiceModel invoice = null;
            try
            {
                if (ModelState.IsValid)
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
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (invoice != null)
                {
                    await AddOrUpdate(invoice);
                }
            }
        }

        public async Task<IActionResult> Create()
        {
            var lastNumberData = await _dbcontext.LastNumber.FindAsync(1);
            var number = lastNumberData.LastNumber + 1;

            var model = new InvoiceViewModel
            {
                Number = number.ToString()
            };

            model.Products.Add(new Product { Quantity = 1, Value = "0,00" });

            await GetViewBagItems();

            return View(model);
        }

        public IActionResult AddProduct([Bind("Products")] InvoiceViewModel invoice)
        {
            invoice.Products.Add(new Product { Quantity = 1, Value = "0,00" });
            return PartialView("Products", invoice);
        }


        public async Task<IActionResult> ShowView(string id, string view)
        {

            if (id == null)
            {
                return NotFound();
            }

            var model = await Find(id);

            await GetViewBagItems();

            if (model == null)
            {
                return NotFound();
            }

            if (view.Equals("LikeModel"))
            {
                var lastNumberData = await _dbcontext.LastNumber.FindAsync(1);
                var number = lastNumberData.LastNumber + 1;
                model.Number = number.ToString();
            }

            return View(view, model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string FindId)
        {
            var invoice = await _dbcontext.Invoices
                                       .FirstOrDefaultAsync(x => x.Invoice.FindId == FindId);

            invoice.Invoice.Inactivate(invoice.Invoice.Created);

            _entity = _dbcontext.Invoices.Update(invoice);

            await _dbcontext.SaveChangesAsync();
            _entity.State = EntityState.Detached;

            return RedirectToAction(nameof(Index));
        }

        private async Task AddOrUpdate(InvoiceModel invoice)
        {
            if (string.IsNullOrEmpty(invoice.FindId))
            {
                invoice.CreateId();
                _entity = _dbcontext.Invoices.Add(new InvoiceData { Invoice = invoice });

                var lastNumber = await _dbcontext.LastNumber.FindAsync(1);
                if (int.TryParse(invoice.Number, out int number) && number > lastNumber.LastNumber)
                {
                    _dbcontext.LastNumber.Update(new LastNumberData { Id = 1, LastNumber = number });
                }
            }
            else
            {
                invoice.IsUpdate(invoice.Created);
                var dataObject = await _dbcontext.Invoices
                                                 .AsNoTracking()
                                                 .SingleOrDefaultAsync(x => x.Invoice.FindId == invoice.FindId);

                dataObject.Invoice = invoice;
                _entity = _dbcontext.Invoices.Update(dataObject);
            }
            await _dbcontext.SaveChangesAsync();
            _entity.State = EntityState.Detached;
        }

        private IActionResult Print(InvoiceModel invoice)
        {
            if (invoice is null)
            {
                throw new InvalidOperationException("Fatura em branco! Não pode ser gerada!");
            }

            var path = _env.WebRootFileProvider.GetFileInfo("LOGO.jpg")?.PhysicalPath;

            return File(new PdfHandler(new Uri(path), invoice).PdfInMemoryStream.ToArray(), MediaTypeNames.Application.Pdf, $"{invoice.Client.Name}_{invoice.Number}.pdf", true);
        }

        private async Task<InvoiceViewModel> Find(string id)
        {
            var dataObject = await _dbcontext.Invoices
                                           .SingleOrDefaultAsync(m => m.Invoice.FindId == id && m.Invoice.IsActive);
            var model = DataToModel(dataObject);

            return model;
        }

        private static InvoiceViewModel DataToModel(InvoiceData dataObject)
        {
            var json = JsonConvert.SerializeObject(dataObject.Invoice);
            var model = JsonConvert.DeserializeObject<InvoiceViewModel>(json);
            model.SelectedItem = dataObject.Invoice.Client.FindId;
            return model;
        }

        private async Task GetViewBagItems()
        {
            var customers = await _dbcontext.Customers
                                    .AsNoTracking()
                                    .Where(x => x.Customer.IsActive)
                                    .Select(x => new SelectListItem { Text = x.Customer.Name, Value = x.Customer.FindId })
                                    .ToListAsync();

            ViewBag.Items = customers;
        }
    }
}