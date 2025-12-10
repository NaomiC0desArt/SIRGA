using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class HistorialCalificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdCalificacion { get; set; }
        [ForeignKey("IdCalificacion")]
        public Calificacion Calificacion { get; set; }

        [Required]
        [MaxLength(450)]
        public string UsuarioModificadorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreUsuarioModificador { get; set; }

        [Required]
        [MaxLength(50)]
        public string RolUsuarioModificador { get; set; }

        [Required]
        public string ValoresAnteriores { get; set; }

        [Required]
        public string ValoresNuevos { get; set; }

        [Required]
        public decimal TotalAnterior { get; set; }

        [Required]
        public decimal TotalNuevo { get; set; }

        [Required]
        [MaxLength(500)]
        public string MotivoModificacion { get; set; }

        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}
