using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class GuardarCalificacionesRequestDto
    {
        [Required]
        public int IdAsignatura { get; set; }

        [Required]
        public int IdCursoAcademico { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        [Required]
        public int IdProfesor { get; set; }

        [Required]
        public string TipoAsignatura { get; set; }

        public List<ComponenteDto> Componentes { get; set; }

        // ✅ USA GuardarCalificacionDto para GUARDAR
        [Required]
        public List<GuardarCalificacionDto> Calificaciones { get; set; }
    }
}
