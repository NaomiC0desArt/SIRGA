using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Identity
{
    public class AuthenticationResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
        public bool MustCompleteProfile { get; set; }
        public string JWToken { get; set; }

        public AuthenticationResponse()
        {
            Roles = new List<string>();
        }
    }
}
