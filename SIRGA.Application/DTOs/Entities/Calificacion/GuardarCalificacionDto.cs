using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class GuardarCalificacionDto
    {
        public int IdEstudiante { get; set; }
        public Dictionary<int, decimal?> Componentes { get; set; }
        public string Observaciones { get; set; }
    }
}
