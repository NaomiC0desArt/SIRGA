using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asistencia
{
    public class ActualizarAsistenciaDto
    {
        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; }

        public string? Observaciones { get; set; }
        public bool RequiereJustificacion { get; set; } = false;
    }
}
