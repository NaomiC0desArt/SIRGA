namespace SIRGA.Web.Models.Periodo
{
    public class PeriodoResponseDto : PeriodoDto
    {
        public int Id { get; set; }
        public string AnioEscolar { get; set; }
        public string NombrePeriodo { get; set; }
        public int DuracionDias { get; set; }
    }
}
