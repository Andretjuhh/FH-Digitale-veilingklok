using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlok.Models.Domain
{
    // Koper Model, represents the Koper table in the database
    public class Koper
    {
        //Since this is a specialization of Account, AccountId is both PK and FK
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("first_name")]
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Column("last_name")]
        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Column("adress")]
        public string? Adress { get; set; }

        [Column("post_code")]
        public string? PostCode { get; set; }
        
        [Column("regio")]
        public string? Regio { get; set; }

        public Account Account { get; set; }

        // Navigation property for the one-to-many relationship with Order
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
