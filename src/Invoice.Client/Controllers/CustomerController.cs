using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationData _context;

        public CustomerController(ApplicationData context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Customer.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.Id == id);
            if(customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public IActionResult Create()
        {
            return View(new Customer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if(ModelState.IsValid)
            {
                if(customer.Id.Equals(0))
                {
                    _context.Add(customer);
                }
                else
                {
                    _context.Update(customer);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                                       .Include(x => x.Address)
                                       .FirstOrDefaultAsync(x => x.Id == id);
            if(customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                                       .FirstOrDefaultAsync(x => x.Id == id);
            if(customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer
                                       .Include(x => x.Address)
                                       .FirstOrDefaultAsync(x => x.Id == id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}