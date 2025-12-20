using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models
{
    public class NewOrder
    {
        [Required]
        public Guid KoperId { get; set; }

        //public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
