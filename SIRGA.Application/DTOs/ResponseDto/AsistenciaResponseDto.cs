

namespace SIRGA.Application.DTOs.ResponseDto
{
    public class AsistenciaResponseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime HoraRegistro { get; set; }
        public string Estado { get; set; }
        public string? Observaciones { get; set; }
        public bool RequiereJustificacion { get; set; }
        public string? Justificacion { get; set; }
        public DateTime? FechaJustificacion { get; set; }

        // Datos del estudiante
        public int IdEstudiante { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteApellido { get; set; }
        public string EstudianteMatricula { get; set; }

        // Datos de la clase
        public int IdClaseProgramada { get; set; }
        public string AsignaturaNombre { get; set; }
        public string ClaseLocation { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        // Datos del profesor
        public int IdProfesor { get; set; }
        public string ProfesorNombre { get; set; }
        public string ProfesorApellido { get; set; }

        // Auditoría
        public string RegistradoPorId { get; set; }
        public DateTime? UltimaModificacion { get; set; }
        public string? ModificadoPorId { get; set; }
        public string? UsuarioJustificacionId { get; set; }
    }
}
