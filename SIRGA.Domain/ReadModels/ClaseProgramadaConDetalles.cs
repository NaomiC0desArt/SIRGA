namespace SIRGA.Domain.ReadModels
{
    public class ClaseProgramadaConDetalles
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public string Location { get; set; }

        // Asignatura
        public int IdAsignatura { get; set; }
        public string AsignaturaNombre { get; set; }

        // Profesor
        public int IdProfesor { get; set; }
        public string ProfesorFirstName { get; set; }
        public string ProfesorLastName { get; set; }

        // Curso Académico
        public int IdCursoAcademico { get; set; }

        // Datos del Grado
        public string GradoNombre { get; set; }

        // Datos de la Sección
        public string SeccionNombre { get; set; }

        // Datos del Año Escolar
        public string AnioEscolarPeriodo
        {
            get; set;
        }

    }
}
