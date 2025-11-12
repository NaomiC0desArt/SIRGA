using SIRGA.Domain.Entities;

namespace SIRGA.Application.DTOs.Entities
{
    public class ClaseProgramadaDto
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public string Location { get; set; }
        public int IdAsignatura { get; set; }
        //public Asignatura Asignatura { get; set; }
        public int IdProfesor { get; set; }
        //public Profesor Profesor { get; set; }
        public int IdCursoAcademico { get; set; }
        //public CursoAcademico CursoAcademico { get; set; }
    }
}
