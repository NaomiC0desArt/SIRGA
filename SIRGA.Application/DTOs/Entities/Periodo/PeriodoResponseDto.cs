namespace SIRGA.Application.DTOs.Entities.Periodo
{
    public class PeriodoResponseDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AnioEscolarId { get; set; }
        public string AnioEscolar { get; set; }
        public string NombrePeriodo { get; set; }
        public int DuracionDias { get; set; }
        public int DuracionSemanas { get; set; }
        public bool EsActivo { get; set; }
        public bool PuedeEditar { get; set; }
    }

}
