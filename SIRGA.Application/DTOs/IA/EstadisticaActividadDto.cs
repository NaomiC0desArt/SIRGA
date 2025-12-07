using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.IA
{
    public class EstadisticaActividadDto
    {
        public int IdActividad { get; set; }
        public string NombreActividad { get; set; }
        public int CantidadInscritos { get; set; }
        public double PorcentajeOcupacion { get; set; }
        public string Categoria { get; set; }
    }
}
