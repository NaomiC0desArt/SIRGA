

using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.UserManagement.Estudiante
{
    public class CreateEstudianteDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; }

        
        [Range(1900, 9999, ErrorMessage = "El año de ingreso debe ser un valor válido (ej: 2025).")]
        public int? YearOfEntry { get; set; } = DateTime.Now.Year;
    }
}
