using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.UserManagement.Profesor
{
    public class CreateProfesorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string Specialty { get; set; }

        // Opcional: Año de ingreso
        public int? YearOfEntry { get; set; }
    }
}
