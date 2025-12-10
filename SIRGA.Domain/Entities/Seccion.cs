using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class Seccion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }  // "A", "B", "C"
        public int CapacidadMaxima { get; set; } = 25;
    }
}
