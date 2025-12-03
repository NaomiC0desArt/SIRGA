
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace SIRGA.Domain.Entities
{
    public class Asistencia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public DateTime HoraRegistro { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } // Presente, Ausente, Tarde, Justificado

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public bool RequiereJustificacion { get; set; } = false;

        [MaxLength(350)]
        public string? Justificacion { get; set; }

        public DateTime? FechaJustificacion { get; set; }

        public string? UsuarioJustificacionId { get; set; } // Admin que justificó

        // Foreign Keys
        public int IdEstudiante { get; set; }
        [ForeignKey("IdEstudiante")]
        public Estudiante Estudiante { get; set; }

        public int IdClaseProgramada { get; set; }
        [ForeignKey("IdClaseProgramada")]
        public ClaseProgramada ClaseProgramada { get; set; }

        public int IdProfesor { get; set; }
        [ForeignKey("IdProfesor")]
        public Profesor Profesor { get; set; }

        // Auditoría
        public string RegistradoPorId { get; set; } // Usuario que registró (Profesor o Admin)
        public DateTime? UltimaModificacion { get; set; }
        public string? ModificadoPorId { get; set; } // Admin que modificó
    }
}
