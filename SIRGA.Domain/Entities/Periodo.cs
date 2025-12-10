using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class Periodo
    {
        public int Id { get; set; }
        public int Numero { get; set; } // 1-4
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int AnioEscolarId { get; set; }
        [ForeignKey("AnioEscolarId")]
        public AnioEscolar AnioEscolar { get; set; }

        [NotMapped]
        public string NombrePeriodo => $"Periodo {Numero}";

        [NotMapped]
        public int DuracionDias => (FechaFin - FechaInicio).Days + 1;
    }
}
