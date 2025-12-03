
namespace SIRGA.Application.DTOs.Entities.Asistencia
{
    public class ClaseDelDiaDto
    {
        public int IdClaseProgramada { get; set; }
        public string AsignaturaNombre { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public string Location { get; set; }
        public int CantidadEstudiantes { get; set; }
        public bool AsistenciaRegistrada { get; set; }
        public int? EstudiantesPresentes { get; set; }
        public int? EstudiantesAusentes { get; set; }
        public int? EstudiantesTarde { get; set; }

        // Info del grado
        public string GradoNombre { get; set; }
        public string GradoSeccion { get; set; }
    }
}
