using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    [Authorize]
    public class AdjustController : Controller
    {
        private readonly ApplicationData _context;
        private readonly ILogger<AdjustController> _logger;

        public AdjustController(ApplicationData context, ILogger<AdjustController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> AddCutomersFromInvoice()
        {
            try
            {
                var invoices = await _context.Invoices
                                              .ToListAsync();
                var customers = invoices.Select(x => new CustomerData { Customer = x.Invoice.Client });
                _context.Customers.AddRange(customers);
                await _context.SaveChangesAsync();
                return RedirectToActionPermanent("Index", "Customer");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> AddFindIdInClinetonOldInvoices()
        {
            try
            {
                var customers = await _context.Customers
                                                   .AsNoTracking()
                                                   .Where(x => x.Customer.IsActive)
                                                   .Select(x => x.Customer)
                                                   .ToListAsync();
                var invoices = await _context.Invoices
                                                  .AsNoTracking()
                                                  .Where(x => x.Invoice.IsActive && x.Invoice.Client.FindId == null)
                                                  .ToListAsync();
                _logger.LogInformation("Funciona");
                invoices.ForEach(invoice =>
                {
                    var customer = customers.FirstOrDefault(c => c.LegalNumber == invoice.Invoice.Client.LegalNumber);
                    invoice.Invoice.Client = customer;
                    _logger.LogInformation(JsonConvert.SerializeObject(invoice));
                });
                return RedirectToActionPermanent("Index", "Invoice");
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                throw;
            }
        }
    }
}