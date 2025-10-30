using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.UserManagement.Profesor
{
    public class UpdateProfesorDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public char? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Province { get; set; }
        public string? Sector { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? Specialty { get; set; }
    }
}
