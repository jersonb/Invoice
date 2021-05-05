using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Client.Models
{
    public class CustomerViewModel
    {
        public string FindId { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "Informe {0}")]
        public string Name { get; set; }

        [DisplayName("CNPJ")]
        [Required(ErrorMessage = "Informe {0}")]
        public string LegalNumber { get; set; }

        [DisplayName("Inscrição Municipal")]
        public string RegionalLegalNumber { get; set; }

        [DisplayName("Endereço")]
        public Address Address { get; set; }
    }
}