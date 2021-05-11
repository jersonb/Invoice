using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    [Authorize]
    public class AdjustController:Controller
    {
        private readonly ApplicationData _context;

        public AdjustController(ApplicationData context)
        {
            _context = context;
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
            return RedirectToActionPermanent("Index","Customer");
          }catch(Exception ex)
          {
              throw ex;
          }
        }
    }
}
