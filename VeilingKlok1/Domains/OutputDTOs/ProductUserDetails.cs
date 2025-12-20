using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeilingKlokApp.Models.OutputDTOs
{
    public class ProductUserDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Size { get; set; }
    }
}
