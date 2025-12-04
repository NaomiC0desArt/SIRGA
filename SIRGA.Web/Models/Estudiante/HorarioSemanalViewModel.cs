namespace SIRGA.Web.Models.Estudiante
{
    public class HorarioSemanalViewModel
    {
        public List<HorarioEstudianteViewModel> HorarioSemanal { get; set; } = new();
        public string PeriodoAcademico { get; set; }
        public string GradoSeccion { get; set; }
    }

    public class HorarioEstudianteViewModel
    {
        public string DiaSemana { get; set; }
        public List<ClaseHorarioViewModel> Clases { get; set; } = new();
    }

    public class ClaseHorarioViewModel
    {
        public int IdClaseProgramada { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string NombreAsignatura { get; set; }
        public string NombreProfesor { get; set; }
        public string Ubicacion { get; set; }
        public bool EsClaseActual { get; set; }
        public bool EsProximaClase { get; set; }

        public string HoraInicioFormateada => HoraInicio.ToString(@"hh\:mm");
        public string HoraFinFormateada => HoraFin.ToString(@"hh\:mm");
    }
}
