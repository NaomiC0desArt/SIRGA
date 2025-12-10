using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Grado
{
    public class UpdateAulaDto
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo de espacio es obligatorio")]
        public int Tipo { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        [Range(1, 200)]
        public int Capacidad { get; set; }

        public bool EstaDisponible { get; set; }
    }
}
