using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice.Client.Models
{
    public class CustomerData
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "jsonb")]
        public CustomerModel Customer { get; set; }
    }
}