using SIRGA.Application.DTOs.Entities.Asistencia;

namespace SIRGA.Web.Models.Asistencia
{
    public class RegistrarAsistenciaMasivaDto
    {
        public int IdClaseProgramada { get; set; }
        public DateTime Fecha { get; set; }
        public List<AsistenciaEstudianteDto> Asistencias { get; set; } = new();
    }
}
