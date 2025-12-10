using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Grado
{
    public class AnioEscolarDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El año de inicio es obligatorio")]
        [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100")]
        public int AnioInicio { get; set; }

        [Required(ErrorMessage = "El año de fin es obligatorio")]
        [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100")]
        public int AnioFin { get; set; }

        public bool Activo { get; set; }
        public string? Periodo { get; set; }
    }

}
