using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asistencia
{
    public class AsistenciaEstudianteDto
    {
        public int IdEstudiante { get; set; }
        public string Estado { get; set; }
        public string? Observaciones { get; set; }
        [MaxLength(350, ErrorMessage = "La justificación no puede exceder 350 caracteres")]
        [MinLength(10, ErrorMessage = "La justificación debe tener al menos 10 caracteres")]
        public string? Justificacion { get; set; }
        public bool RequiereJustificacion { get; set; } = false;
    }
}
