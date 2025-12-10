using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Web.Models.Seccion
{
    public class CreateSeccionDto
    {
        [Required(ErrorMessage = "El nombre de la sección es obligatorio")]
        [StringLength(10)]
        public string Nombre { get; set; }

        [Range(1, 100, ErrorMessage = "La capacidad debe estar entre 1 y 100")]
        public int CapacidadMaxima { get; set; } = 25;
    }
}
