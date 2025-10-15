namespace VeilingKlokApp.Models
{
    public class NewKwekerAccount
    {
        // Basic information layout of a new Kweker account
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  
        public DateTime CreatedAt { get; set; }     
        public string Telephone { get; set; }
        public string? Adress { get; set; }
        public string? Regio { get; set; }
        public string? KvkNumber { get; set; }
    }
}
