using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Client.Models
{
    public class Product
    {
        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Informe a {0}")]
        public string Description { get; set; }

        [DisplayName("Quantidade")]
        [Required(ErrorMessage = "Informe a {0}")]
        public int Quantity { get; set; }

        public decimal UnitaryValue => decimal.Parse(Value);

        public decimal TotalValue => UnitaryValue * Quantity;

        [DisplayName("Valor Unitário")]
        [Required(ErrorMessage = "Informe a {0}")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [RegularExpression(@"^(?=.)(\d{1,3}(.\d{3})*)?(\,\d+)?$", ErrorMessage = "escreva no formato 1.010,10")]
        public string Value { get; set; }
    }
}