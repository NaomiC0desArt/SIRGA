using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Grado
{
    public class CreateGradoDto
    {
        [Required(ErrorMessage = "El nombre del grado es requerido")]
        [StringLength(50, ErrorMessage = "El nombre del grado no puede exceder 50 caracteres")]
        [Display(Name = "Nombre del Grado")]
        public string GradeName { get; set; }

        [Required(ErrorMessage = "El nivel educativo es requerido")]
        [Range(1, 2, ErrorMessage = "Seleccione un nivel válido")]
        [Display(Name = "Nivel Educativo")]
        public int Nivel { get; set; }
    }
}
