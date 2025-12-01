namespace SIRGA.Application.DTOs.Entities
{
    public class AnnualGradeDto
    {
        public int Id { get; set; }
        public int CalificacionId { get; set; }
        public int PeriodoId { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P2 { get; set; }
        public decimal? P3 { get; set; }
        public decimal? P4 { get; set; }
        public decimal Total { get; set; }
    }
}
