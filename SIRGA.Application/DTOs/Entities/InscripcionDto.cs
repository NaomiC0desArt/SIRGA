using SIRGA.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities
{
    public class InscripcionDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El ID del estudiante es requerido")]
        public int IdEstudiante { get; set; }

        [Required(ErrorMessage = "El ID del curso académico es requerido")]
        public int IdCursoAcademico { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
