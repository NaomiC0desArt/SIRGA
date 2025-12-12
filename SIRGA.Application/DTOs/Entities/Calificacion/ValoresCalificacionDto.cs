namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class ValoresCalificacionDto
    {
        public Dictionary<string, decimal?> Componentes { get; set; }
        public decimal? Total { get; set; }
    }
}
