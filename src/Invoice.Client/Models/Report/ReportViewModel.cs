using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Client.Models
{
    public class ReportViewModel
    {
        [DisplayName("Data Inicial")]
        [Required(ErrorMessage ="Informe {0}")]
        public DateTime? Initial { get; set; }
        [DisplayName("Data Final")]
        public DateTime? Final { get; set; }
    }
}