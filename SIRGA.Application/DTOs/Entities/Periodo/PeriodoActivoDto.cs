
namespace SIRGA.Application.DTOs.Entities.Periodo
{
    public class PeriodoActivoDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NombrePeriodo { get; set; }
        public int DiasRestantes { get; set; }
        public string AnioEscolar { get; set; }
    }
}
