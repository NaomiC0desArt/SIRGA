
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Asistencia
{
    public class AsistenciaEstudianteDto
    {
        public int IdEstudiante { get; set; }

        [Required]
        [RegularExpression("^(Presente|Ausente|Tarde|Justificado)$")]
        public string Estado { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        [MaxLength(500, ErrorMessage = "La justificación no puede exceder 500 caracteres")]
        [MinLength(10, ErrorMessage = "La justificación debe tener al menos 10 caracteres")]
        public string? Justificacion { get; set; }

        public bool RequiereJustificacion { get; set; } = false;
    }
}
