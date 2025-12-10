namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class CalificacionPorPeriodoDto
    {
        public int NumeroPeriodo { get; set; }
        public Dictionary<string, decimal?> Componentes { get; set; }
        public decimal? Total { get; set; }
        public bool Publicada { get; set; }
    }
}
