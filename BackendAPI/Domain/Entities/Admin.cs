using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    [Table("Admin")]
    public class Admin : Account
    {
        [NotMapped]
        public override AccountType AccountType => AccountType.Admin;

        private Admin()
            : base() { }

        public Admin(string email, Password password)
            : base(email, password) { }
    }
}
