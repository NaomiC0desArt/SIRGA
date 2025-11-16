using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Grado
{
    public class CreateGradoDto
    {
        [Required(ErrorMessage = "El nombre del grado es requerido")]
        [StringLength(50, ErrorMessage = "El nombre del grado no puede exceder 50 caracteres")]
        [Display(Name = "Nombre del Grado")]
        public string GradeName { get; set; }

        [Required(ErrorMessage = "La sección es requerida")]
        [StringLength(10, ErrorMessage = "La sección no puede exceder 10 caracteres")]
        [Display(Name = "Sección")]
        public string Section { get; set; }

        [Required(ErrorMessage = "El límite de estudiantes es requerido")]
        [Range(1, 100, ErrorMessage = "El límite debe estar entre 1 y 100 estudiantes")]
        [Display(Name = "Límite de Estudiantes")]
        public int StudentsLimit { get; set; } = 25;
    }
}
