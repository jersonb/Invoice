using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Invoice.Client.Models
{
    public class Address
    {
        [DisplayName("Logradouro")]
        [Required(ErrorMessage = "Informe {0}")]
        public string Street { get; set; }

        [DisplayName("Numero")]
        [Required(ErrorMessage = "Informe {0}")]
        public string Number { get; set; }

        [DisplayName("Estado")]
        [Required(ErrorMessage = "Informe {0}")]
        public string State { get; set; }

        [DisplayName("CEP")]
        public string ZipCode { get; set; }

        [DisplayName("Município")]
        [Required(ErrorMessage = "Informe {0}")]
        public string Region { get; set; }
    }
}