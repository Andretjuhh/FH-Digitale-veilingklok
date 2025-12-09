using System.ComponentModel.DataAnnotations;

namespace VeilingKlokKlas1Groep2.Models.InputDTOs
{
    /// <summary>
    /// DTO for updating account information.
    /// Supports all account types (Koper, Kweker, Veilingmeester).
    /// Only provided fields will be updated.
    /// </summary>
    public class UpdateAccountRequest
    {
        // Shared fields
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public string? Adress { get; set; }
        public string? Regio { get; set; }

        // KOPER fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PostCode { get; set; }

        // KWEKER fields
        public string? Name { get; set; }
        public string? Telephone { get; set; }
        public string? KvkNumber { get; set; }

        // VEILINGMEESTER fields
        public string? AuthorisatieCode { get; set; }
    }
}
