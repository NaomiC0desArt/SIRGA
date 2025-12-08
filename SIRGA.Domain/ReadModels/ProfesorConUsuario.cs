using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.ReadModels
{
    public class ProfesorConUsuario
    {
        public int Id { get; set; }
        public string Specialty { get; set; }
        public string ApplicationUserId { get; set; }

        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string NombreCompleto => $"{FirstName} {LastName}";
    }
}
