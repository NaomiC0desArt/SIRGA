using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.UserManagement.Estudiante
{
    public class UpdateEstudianteDto
    {
        
        [StringLength(100)]
        public string? FirstName { get; set; }

     
        [StringLength(100)]
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }

        
        public char? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Province { get; set; }
        public string? Sector { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
