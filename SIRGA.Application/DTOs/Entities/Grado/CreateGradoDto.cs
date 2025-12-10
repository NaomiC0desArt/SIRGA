using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Grado
{
    public class CreateGradoDto
    {
        [Required(ErrorMessage = "Favor ingresar un nombre")]
        public string GradeName { get; set; }

        [Required(ErrorMessage = "Favor seleccionar un nivel educativo")]
        public int Nivel { get; set; }  // 1=Primaria, 2=Secundaria
    }
}
