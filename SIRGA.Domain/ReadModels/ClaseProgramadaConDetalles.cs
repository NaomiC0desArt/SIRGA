namespace SIRGA.Domain.ReadModels
{
    public class ClaseProgramadaConDetalles
    {
        public int Id { get; set; }
        public int IdAsignatura { get; set; }
        public int IdProfesor { get; set; }
        public int IdCursoAcademico { get; set; }


        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public string Location { get; set; }


        public string AsignaturaNombre { get; set; }
        public string ProfesorFirstName { get; set; }
        public string ProfesorLastName { get; set; }
        public string GradoNombre { get; set; }
        public string GradoSeccion { get; set; }
        public string SchoolYear { get; set; }
    }
}
