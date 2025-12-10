using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class PublicarCalificacionesDto
    {
        [Required]
        public int IdAsignatura { get; set; }

        [Required]
        public int IdCursoAcademico { get; set; }

        [Required]
        public int IdPeriodo { get; set; }
    }
}
