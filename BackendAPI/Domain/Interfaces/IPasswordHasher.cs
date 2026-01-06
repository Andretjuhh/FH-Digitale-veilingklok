using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPasswordHasher
    {
        public string Hash(string rawPassword);
        public bool Verify(string providedPassword, string hashedPassword);
    }
}
