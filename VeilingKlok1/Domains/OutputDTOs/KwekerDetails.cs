namespace VeilingKlokApp.Models
{
    public class KwekerDetails
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Telephone { get; set; }
        public string? Adress { get; set; }
        public string? Regio { get; set; }
        public string? KvkNumber { get; set; }
        public string? PostCode { get; set; }
    }
}
