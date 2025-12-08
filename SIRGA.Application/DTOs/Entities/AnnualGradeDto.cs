using SIRGA.Domain.Entities;

namespace SIRGA.Application.DTOs.Entities
{
    public class AnnualGradeDto
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int AsignatuaId { get; set; }
        public string AsignaturaName { get; set; }
        public int CursoId { get; set; }
        public int CalificacionId { get; set; }
        public int PeriodoId { get; set; }
        public Periodo Periodo { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P2 { get; set; }
        public decimal? P3 { get; set; }
        public decimal? P4 { get; set; }
        public decimal Total { get; set; }
    }
}
