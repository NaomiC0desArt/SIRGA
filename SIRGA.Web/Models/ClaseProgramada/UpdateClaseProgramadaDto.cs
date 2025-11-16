namespace SIRGA.Web.Models.ClaseProgramada
{
    public class UpdateClaseProgramadaDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WeekDay { get; set; }
        public string Location { get; set; }
        public int IdAsignatura { get; set; }
        public int IdProfesor { get; set; }
        public int IdCursoAcademico { get; set; }
    }
}
