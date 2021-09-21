using jwt_authentication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwt_authentication.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
