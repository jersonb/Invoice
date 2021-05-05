using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice.Client.Models
{
    public class InvoiceData
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "jsonb")]
        public InvoiceModel Invoice { get; set; }
    }
}