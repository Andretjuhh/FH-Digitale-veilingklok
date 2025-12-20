using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeilingKlokApp.Types;

namespace VeilingKlokKlas1Groep2.Models.OutputDTOs
{
    public class AccountDetails
    {
        public required Guid AccountId { get; set; }
        public required AccountType AccountType { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Regio { get; set; }
        public string? Adress { get; set; }
        public string? PostCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? KvkNumber { get; set; }
    }
}
