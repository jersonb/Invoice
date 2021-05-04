using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Invoice.Client.Models
{
    public class CreateInvoiceViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<SelectListItem> Itens {get;set;}
        public InvoiceModel Invoice { get; set; }

        private void SetItens()
        {
            Itens = new List<SelectListItem>();

          //  Itens = 
            var a =Customer.Where(x => x.);

        }
    }
}