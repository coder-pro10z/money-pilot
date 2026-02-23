using MoneyPilot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

namespace MoneyPilot.Application.Interfaces
{
    public interface ILoginTokenService
    {
        //Add LoginToken specific methods here without implementation\
        Task<string?> GenerateAutoLoginTokenAsync();
        Task<string> GenerateJwtTokenAsync(AppUser user);
    }
}
