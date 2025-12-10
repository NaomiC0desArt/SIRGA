namespace SIRGA.Web.Models.Periodo
{
    public class CreatePeriodoDto
    {
        public int Numero { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AnioEscolarId { get; set; }
    }
}
