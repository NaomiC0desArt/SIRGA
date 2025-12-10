namespace SIRGA.Web.Models.ClaseProgramada
{
    public class ClaseProgramadaDto
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WeekDay { get; set; }
        public string Location { get; set; }

        // IDs
        public int IdAsignatura { get; set; }
        public int IdProfesor { get; set; }
        public int IdCursoAcademico { get; set; }

        // Detalles para mostrar
        public string AsignaturaNombre { get; set; }
        public string ProfesorNombre { get; set; }
        public string CursoAcademicoNombre { get; set; }
    }
}
