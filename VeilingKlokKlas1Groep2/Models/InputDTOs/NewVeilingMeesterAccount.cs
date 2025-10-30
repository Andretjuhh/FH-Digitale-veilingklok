using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace VeilingKlokKlas1Groep2.Models
{
    public class NewVeilingMeesterAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Regio { get; set; }
        public int AurthorisatieCode { get; set; }
    }
}