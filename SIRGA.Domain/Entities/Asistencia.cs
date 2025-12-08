
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
        public string Estado { get; set; }

        [MaxLength(125)]
        public string? Observaciones { get; set; }

        public bool RequiereJustificacion { get; set; } = false;

        [MaxLength(350)]
        public string? Justificacion { get; set; }

        public DateTime? FechaJustificacion { get; set; }

        public string? UsuarioJustificacionId { get; set; }


        public int IdEstudiante { get; set; }
        [ForeignKey("IdEstudiante")]
        public Estudiante Estudiante { get; set; }

        public int IdClaseProgramada { get; set; }
        [ForeignKey("IdClaseProgramada")]
        public ClaseProgramada ClaseProgramada { get; set; }

        public int IdProfesor { get; set; }
        [ForeignKey("IdProfesor")]
        public Profesor Profesor { get; set; }


        public string RegistradoPorId { get; set; } // persona que registro la asistencia
        public DateTime? UltimaModificacion { get; set; }
        public string? ModificadoPorId { get; set; } // admin que modificó la asistencia
    }
}
