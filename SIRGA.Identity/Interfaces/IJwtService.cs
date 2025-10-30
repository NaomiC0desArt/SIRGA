using SIRGA.Identity.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Identity.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user, List<string> roles);
    }
}
