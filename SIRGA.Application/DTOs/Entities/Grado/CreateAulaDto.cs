using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Grado
{
    public class CreateAulaDto
    {
        
        [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo de espacio es obligatorio")]
        public int Tipo { get; set; } // TipoEspacio enum

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        [Range(1, 200, ErrorMessage = "La capacidad debe estar entre 1 y 200")]
        public int Capacidad { get; set; }

        public bool EstaDisponible { get; set; } = true;
    }
}
