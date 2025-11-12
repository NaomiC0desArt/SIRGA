using SIRGA.Domain.Entities;

namespace SIRGA.Application.DTOs.ResponseDto
{
    public class InscripcionResponseDto
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        public Estudiante Estudiante { get; set; }
        public int IdCursoAcademico { get; set; }
        public CursoAcademico CursoAcademico { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
