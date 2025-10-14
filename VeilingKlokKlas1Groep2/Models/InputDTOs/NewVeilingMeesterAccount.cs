using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace VeilingKlokKlas1Groep2.Models
{
    public class NewVeilingMeesterAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SoortVeiling { get; set; }
    }
}