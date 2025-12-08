

using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Asistencia
{
    public class RegistrarAsistenciaMasivaDto
    {
        [Required]
        public int IdClaseProgramada { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public List<AsistenciaEstudianteDto> Asistencias { get; set; }
    }

    public class JustificarAsistenciaDto
    {
        [Required]
        [MaxLength(350)]
        public string Justificacion { get; set; }
    }
}