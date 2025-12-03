using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asistencia
{
    public class RegistrarAsistenciaDto
    {
        public int IdEstudiante { get; set; }
        public int IdClaseProgramada { get; set; }
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; }

        public string? Observaciones { get; set; }
        public bool RequiereJustificacion { get; set; } = false;
    }
}
