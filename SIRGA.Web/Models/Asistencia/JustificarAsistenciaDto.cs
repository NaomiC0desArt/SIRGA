using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asistencia
{
    public class JustificarAsistenciaDto
    {
        [Required(ErrorMessage = "La justificación es requerida")]
        [StringLength(350, ErrorMessage = "La justificación no puede exceder 350 caracteres")]
        public string Justificacion { get; set; }
    }
}
