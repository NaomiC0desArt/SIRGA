namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class PeriodoCalificacionViewDto
    {
        public int NumeroPeriodo { get; set; }
        public Dictionary<string, decimal?> Componentes { get; set; }
        public decimal? Total { get; set; }
        public bool Publicada { get; set; }
    }
}
