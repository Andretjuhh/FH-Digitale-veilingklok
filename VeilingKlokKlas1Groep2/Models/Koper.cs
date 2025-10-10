using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject_Klas1_Groep2.Models
{
    public class Koper
    {
        // [Key]
        // [Column("id")]
        // public int Id { get; set; }

        [Column("account_id")]
        //[ForeignKey(nameof(Account))]
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
    }
}
