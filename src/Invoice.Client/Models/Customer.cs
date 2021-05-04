using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Invoice.Client.Models
{
    public class Customer
    {
        public Customer()
        {
            Address = new Address();
        }

        [Key]
        public int Id { get; set; }

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