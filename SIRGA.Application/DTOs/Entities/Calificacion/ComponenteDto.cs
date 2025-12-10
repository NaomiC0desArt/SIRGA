using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class ComponenteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal ValorMaximo { get; set; }
        public int Orden { get; set; }
    }
}
