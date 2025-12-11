using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Calificacion
{
    public class EditarCalificacionDto
    {
        [Required]
        public int IdCalificacion { get; set; }

        [Required]
        [MaxLength(500)]
        public string MotivoModificacion { get; set; }

        [Required]
        public GuardarCalificacionDto NuevaCalificacion { get; set; }
    }
}
