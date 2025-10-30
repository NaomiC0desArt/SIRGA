using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Estudiante
{
    public class CreateEstudianteDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Range(2000, 2099, ErrorMessage = "El año de ingreso debe estar entre 2000 y 2099")]
        [Display(Name = "Año de Ingreso")]
        public int? YearOfEntry { get; set; }
    }
}
