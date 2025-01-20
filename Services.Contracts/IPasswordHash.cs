using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IPasswordHash
    {
        string Encrypt(string password, out string salt);
        bool Decrypt(string password, string storedHash, string storedSalt);
    }
}
