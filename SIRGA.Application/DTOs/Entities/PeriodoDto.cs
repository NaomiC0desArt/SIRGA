using SIRGA.Domain.Enum;

namespace SIRGA.Application.DTOs.Entities
{
    public class PeriodoDto
    {
        public int Id { get; set; }
        public NumeroPeriodo Numero { get; set; } // 1–4
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AnioEscolarId { get; set; }
        /*[ForeignKey("AnioEscolarId")]
        public AnioEscolar AnioEscolar { get; set; }*/
    }
}
