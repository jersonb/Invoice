using System.ComponentModel.DataAnnotations;

namespace Invoice.Client.Models
{
    public class LastNumberData
    {
         [Key]
        public int Id { get; set; }
        public int LastNumber { get; set; }
    }
}