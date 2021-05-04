using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Invoice.Client.Models
{
    public class InvoiceModel
    {
        public InvoiceModel()
        {
            Products = new List<Product>();
            Client = new Customer();
            CreateId();
        }

        public string FindId { get; set; }

        public void CreateId()
        {
            FindId = Guid.NewGuid().ToString();
            IsActive = true;
            Created = DateTime.Now;
            IsUpdate();
        }

        public bool IsActive { get; set; }

        public void Inactivate()
        {
            IsActive = false;
            IsUpdate();
        }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public void IsUpdate()
            => Updated = DateTime.Now;

        [DisplayName("Número")]
        [Required(ErrorMessage = "Informe {0}")]
        public string Number { get; set; }

        [DisplayName("Empenho")]
        public string Commitment { get; set; }

        [DisplayName("Data")]
        public DateTime? Date { get; set; }

        [DisplayName("Cliente")]
        public Customer Client { get; set; }

        [DisplayName("Período do Serviço")]
        [MaxLength(30,ErrorMessage ="máximo de {1} para {0}")]
        public string ServicePeriod { get; set; }

        [DisplayName("Produtos")]
        public List<Product> Products { get; private set; }

        public void AddProduct(Product product)
        {
            if(string.IsNullOrEmpty(product.Description))
            {
                Products.Add(product);
            }
        }

        public void AddProduct(List<Product> products)
           => Products.AddRange(products);

        public decimal Total
            => Products.Sum(x => x.TotalValue);

        [DisplayName("Observação")]
        public string Observation { get; set; }
    }
}