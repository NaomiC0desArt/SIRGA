

using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Asistencia
{
     public class RegistrarAsistenciaDto
    {

        [Required(ErrorMessage = "El ID del estudiante es requerido")]
        public int IdEstudiante { get; set; }

        [Required(ErrorMessage = "El ID de la clase es requerido")]
        public int IdClaseProgramada { get; set; }

        [Required(ErrorMessage = "Es necesario que ingrese la fecha.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [RegularExpression("^(Presente|Ausente|Tarde|Justificado)$",
            ErrorMessage = "El estado debe ser: Presente, Ausente o Tarde")]
        public string Estado { get; set; }

        [MaxLength(125, ErrorMessage = "Las observaciones no pueden exceder 125 caracteres")]
        public string? Observaciones { get; set; }

        [MaxLength(350, ErrorMessage = "La justificación no puede exceder 500 caracteres")]
        [MinLength(10, ErrorMessage = "La justificación debe tener al menos 10 caracteres")]
        public string? Justificacion { get; set; }

        public bool RequiereJustificacion { get; set; }
    }
}
