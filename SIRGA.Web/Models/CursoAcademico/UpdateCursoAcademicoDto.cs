using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.CursoAcademico
{
    public class UpdateCursoAcademicoDto
    {
        [Required(ErrorMessage = "Debe seleccionar un grado")]
        [Display(Name = "Grado")]
        public int IdGrado { get; set; }

        [Required(ErrorMessage = "El año escolar es requerido")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "El formato debe ser YYYY-YYYY (ej: 2024-2025)")]
        [Display(Name = "Año Escolar")]
        public string SchoolYear { get; set; }
    }
}
