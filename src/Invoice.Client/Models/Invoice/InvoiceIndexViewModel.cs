using System;
using System.ComponentModel;

namespace Invoice.Client.Models
{
    public class InvoiceIndexViewModel
    {
        public string FindId { get; set; }

        [DisplayName("Número")]
        public string Number { get; set; }

        [DisplayName("Data")]
        public DateTime? Date { get; set; }

        [DisplayName("Cliente")]
        public string Customer { get; set; }

        [DisplayName("Valor")]
        public decimal Total { get; set; }
    }
}