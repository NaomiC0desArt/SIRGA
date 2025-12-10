using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class CapturaMasivaDto
    {
        [Required]
        public int IdAsignatura { get; set; }

        [Required]
        public int IdCursoAcademico { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        [Required]
        public int IdProfesor { get; set; }

        [Required]
        public string TipoAsignatura { get; set; }

        public List<ComponenteDto> Componentes { get; set; }

        public List<CalificacionEstudianteDto> Calificaciones { get; set; }
    }
}
