
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

        public bool RequiereJustificacion { get; set; } = false;
    }
}
