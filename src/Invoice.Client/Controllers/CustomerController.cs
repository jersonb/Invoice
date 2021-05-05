using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationData _context;
        private EntityEntry<CustomerData> _entity;

        public CustomerController(ApplicationData context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Customers
                                    .AsNoTracking()
                                    .Where(x => x.Customer.IsActive)
                                    .Select(x => DataToModel(x))
                                    .ToListAsync();
            return View(model);
        }

        private async Task<CustomerViewModel> Find(string id)
        {
            var dataObject = await _context.Customers
                                           .SingleOrDefaultAsync(m => m.Customer.FindId == id && m.Customer.IsActive);
            var model = DataToModel(dataObject);

            return model;
        }

        private static CustomerViewModel DataToModel(CustomerData dataObject)
        {
            var json = JsonConvert.SerializeObject(dataObject.Customer);
            var model = JsonConvert.DeserializeObject<CustomerViewModel>(json);
            return model;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(viewModel);
                var model = JsonConvert.DeserializeObject<CustomerModel>(json);

                if(string.IsNullOrEmpty(model.FindId))
                {
                    model.CreateId();
                    var data = new CustomerData { Customer = model };
                    _entity = _context.Add(data);
                }
                else
                {
                    var dataObject = await _context.Customers.SingleOrDefaultAsync(x => x.Customer.FindId == model.FindId);

                    model.IsUpdate(dataObject.Customer.Created);

                    dataObject.Customer = model;

                    _entity = _context.Update(dataObject);
                }

                await _context.SaveChangesAsync();
                _entity.State = EntityState.Detached;

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string FindId)
        {
            var customer = await _context.Customers
                                       .FirstOrDefaultAsync(x => x.Customer.FindId == FindId);

            customer.Customer.Inactivate(customer.Customer.Created);

            _entity = _context.Customers.Update(customer);

            await _context.SaveChangesAsync();
            _entity.State = EntityState.Detached;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ShowView(string id, string view)
        {
            if(id == null)
            {
                return NotFound();
            }

            var model = await Find(id);

            if(model == null)
            {
                return NotFound();
            }

            return View(view, model);
        }
    }
}