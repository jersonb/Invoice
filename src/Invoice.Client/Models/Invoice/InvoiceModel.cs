using System;
using System.Collections.Generic;
using System.Linq;

namespace Invoice.Client.Models
{
    public class InvoiceModel : DataObject
    {
        public InvoiceModel() : base()
        {
            Products = new List<Product>();
            Client = new CustomerModel();
        }

        public string Number { get; set; }

        public string Commitment { get; set; }

        public DateTime Date { get; set; }
        public string ServicePeriod { get; set; }
        public string Observation { get; set; }

        public CustomerModel Client { get; set; }

        public List<Product> Products { get; set; }

        public void AddProduct(Product product)
        {
            if(string.IsNullOrEmpty(product.Description))
            {
                Products.Add(product);
            }
        }

        public decimal Total
            => Products.Sum(x => x.TotalValue);
    }
}