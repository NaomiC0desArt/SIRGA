using SIRGA.Application.DTOs.Entities.Calificacion;

namespace SIRGA.Web.Models.Calificacion
{
    public class CapturaMasivaDto
    {
        public int IdAsignatura { get; set; }
        public int IdCursoAcademico { get; set; }
        public int IdPeriodo { get; set; }
        public int IdProfesor { get; set; }

        // AGREGAR ESTAS PROPIEDADES:
        public string TipoAsignatura { get; set; }
        public List<ComponenteDto> Componentes { get; set; }

        public List<CalificacionEstudianteDto> Calificaciones { get; set; }
    }
}
