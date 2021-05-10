using Converter;
using Invoice.Client.Data;
using Invoice.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Invoice.Client.Controllers
{
    [Authorize]
    public class MyReportController : Controller
    {
        private readonly ApplicationData _context;

        public MyReportController(ApplicationData context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ReportViewModel model)
        {
            if(ModelState.IsValid)
            {
                model.Final ??= DateTime.Now.Date;

                if(DateTime.Compare(model.Initial.Value.Date, model.Final.Value.Date) == 1)
                {
                    ViewBag.Messages = "A data inicial deve ser menor que a final";
                    return View(nameof(Index), model);
                }

                var invoices = await _context.Invoices
                                                .AsNoTracking()
                                                .Where(x => x.Invoice.IsActive
                                                                     && x.Invoice.Date.Date >= model.Initial.Value.Date
                                                                     && x.Invoice.Date.Date <= model.Final.Value.Date)
                                                .Select(x => new
                                                {
                                                    Número = x.Invoice.Number.PadLeft(6, '0'),
                                                    Data = $"{x.Invoice.Date:dd/MM/yyyy}",
                                                    Empenho = (x.Invoice.Commitment ?? ""),
                                                    Período_Seriço = (x.Invoice.ServicePeriod ?? ""),
                                                    Clinete_Nome = x.Invoice.Client.Name,
                                                    Cliente_CNPJ = x.Invoice.Client.LegalNumber,
                                                    Total = $"{x.Invoice.Total:C2}",
                                                })
                                                .ToListAsync();
                JsonToXlsx xlsx = JsonConvert.SerializeObject(invoices);

                return File(xlsx.MemoryStream.ToArray(), MediaTypeNames.Application.Octet, $"Relatório_de_Faturas-{model.Initial:dd/MM/yyyy}_{model.Final:dd/MM/yyyy}.xlsx", true);
            }
            return View(nameof(Index), model);
        }
    }
}