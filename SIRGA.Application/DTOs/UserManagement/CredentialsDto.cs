using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.UserManagement
{
    public class CredentialsDto
    {
        public string Email { get; set; }
        public string TemporaryPassword { get; set; }
        public string Matricula { get; set; }
    }
}
