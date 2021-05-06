using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Client.Models
{
    public class InvoiceViewModel
    {
        public InvoiceViewModel()
        {
            Products = new List<Product>();
        }

        public string FindId { get; set; }

        [DisplayName("N�mero")]
        [Required(ErrorMessage = "Informe {0}")]
        public string Number { get; set; }

        [DisplayName("Cliente")]
        [Required(ErrorMessage = "Informe {0}")]
        public string SelectedItem { get; set; }

        [DisplayName("Empenho")]
        public string Commitment { get; set; }

        [DisplayName("Data")]
        [Required(ErrorMessage = "Informe {0}")]
        public DateTime? Date { get; set; }

        [DisplayName("Per�odo do Servi�o")]
        [MaxLength(30, ErrorMessage = "m�ximo de {1} para {0}")]
        public string ServicePeriod { get; set; }

        [DisplayName("Observa��o")]
        public string Observation { get; set; }

        [DisplayName("Produtos")]
        [Required(ErrorMessage = "Informe {0}")]
        public List<Product> Products { get; set; }
    }
}