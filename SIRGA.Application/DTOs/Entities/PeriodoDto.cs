using SIRGA.Domain.Entities;
using SIRGA.Domain.Enum;

namespace SIRGA.Application.DTOs.Entities
{
    public class PeriodoDto
    {
        public int Id { get; set; }
        public int Numero { get; set; } // 1–4
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AnioEscolarId { get; set; }
        public string? AnioEscolar { get; set; }
    }
}
