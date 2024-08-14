using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary.Models.TokenOptionsModel
{
    public interface ITokenOptionsModel
    {
        public SymmetricSecurityKey GetSymmetricSecurityKey();
    }
}
