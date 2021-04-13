using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Invoice.Client.Models
{
    public class Client
    {
        public Client()
        {
            Address = new Address();
        }

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