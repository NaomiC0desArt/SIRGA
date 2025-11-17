using SIRGA.Domain.Entities;

namespace SIRGA.Application.DTOs.ResponseDto
{
    public class ClaseProgramadaResponseDto
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WeekDay { get; set; }
        public string Location { get; set; }
        public int IdAsignatura { get; set; }
        public string AsignaturaNombre { get; set; } // ← String, no objeto

        public int IdProfesor { get; set; }
        public string ProfesorNombre { get; set; } // ← String, no objeto

        public int IdCursoAcademico { get; set; }
        public string CursoAcademicoNombre { get; set; }
    }
}
