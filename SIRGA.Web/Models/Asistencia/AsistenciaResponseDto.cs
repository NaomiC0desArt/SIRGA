namespace SIRGA.Web.Models.Asistencia
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

        public int IdEstudiante { get; set; }
        public string EstudianteNombre { get; set; }
        public string EstudianteApellido { get; set; }
        public string EstudianteMatricula { get; set; }
        public string EstudianteNombreCompleto => $"{EstudianteApellido}, {EstudianteNombre}";

        public int IdClaseProgramada { get; set; }
        public string AsignaturaNombre { get; set; }
        public string ClaseLocation { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public int IdProfesor { get; set; }
        public string ProfesorNombre { get; set; }
        public string ProfesorApellido { get; set; }
        public string ProfesorNombreCompleto => $"{ProfesorApellido}, {ProfesorNombre}";

        public string RegistradoPorId { get; set; }
        public DateTime? UltimaModificacion { get; set; }
        public string? ModificadoPorId { get; set; }
        public string? UsuarioJustificacionId { get; set; }

        public string EstadoBadgeClass => Estado switch
        {
            "Presente" => "badge bg-success",
            "Ausente" => "badge bg-danger",
            "Tarde" => "badge bg-warning text-dark",
            "Justificado" => "badge bg-info",
            _ => "badge bg-secondary"
        };
    }
}
