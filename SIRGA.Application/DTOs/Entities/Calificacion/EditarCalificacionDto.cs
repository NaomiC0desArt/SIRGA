using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class EditarCalificacionDto
    {
        [Required]
        public int IdEstudiante { get; set; }

        [Required]
        public int IdAsignatura { get; set; }

        [Required]
        [Range(1, 4)]
        public int IdPeriodo { get; set; }

        [Required]
        public Dictionary<string, decimal?> Componentes { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string MotivoEdicion { get; set; }
    }
}
