using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asignatura
{
    public class UpdateAsignaturaDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre de la Asignatura")]
        public string Nombre { get; set; }

        [StringLength(10, ErrorMessage = "El código no puede exceder 10 caracteres")]
        [Display(Name = "Código de la Asignatura")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(125, ErrorMessage = "La descripción no puede exceder los 125 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de asignatura es requerido")]
        [Display(Name = "Tipo de Asignatura")]
        public string TipoAsignatura { get; set; }
    }
}
