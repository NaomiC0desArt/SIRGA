using SIRGA.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class Periodo
    {
        [Key]
        public int Id { get; set; }
        public NumeroPeriodo Numero { get; set; } // 1–4
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AnioEscolarId { get; set; }
        [ForeignKey("AnioEscolarId")]
        public AnioEscolar AnioEscolar { get; set; }
    }
}
