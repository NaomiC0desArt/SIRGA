using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Asistencia
{
    public class ActualizarAsistenciaDto
    {
        [Required]
        [RegularExpression("^(Presente|Ausente|Tarde|Justificado)$")]
        public string Estado { get; set; }

        [MaxLength(125)]
        public string? Observaciones { get; set; }

        public bool RequiereJustificacion { get; set; } = false;
    }
}
