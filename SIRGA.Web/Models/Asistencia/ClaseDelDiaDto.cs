namespace SIRGA.Web.Models.Asistencia
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
        public int? EstudiantesJustificados { get; set; }

        public string GradoNombre { get; set; }
        public string GradoSeccion { get; set; }
        public string GradoCompleto => $"{GradoNombre} - {GradoSeccion}";

        public string HorarioFormateado => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";

        public string EstadoBadgeClass => AsistenciaRegistrada
            ? "badge bg-success"
            : "badge bg-warning text-dark";

        public string EstadoTexto => AsistenciaRegistrada
            ? "Registrada"
            : "Pendiente";
    }
}
