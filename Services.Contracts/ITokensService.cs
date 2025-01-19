using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface ITokensService
    {
        string JwtToken(List<Claim> claims);
    }
}
