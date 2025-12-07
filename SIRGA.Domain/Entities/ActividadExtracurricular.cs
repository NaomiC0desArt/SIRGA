

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Domain.Entities
{
    public class ActividadExtracurricular
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [MaxLength(30)]
        public string Categoria { get; set; } // Curso, Actividad/Voluntariado, Club

        [MaxLength(300)]
        public string Requisitos { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Required]
        public DayOfWeek DiaSemana { get; set; }

        [MaxLength(100)]
        public string Ubicacion { get; set; }

        [Required]
        [MaxLength(7)] // formato #RRGGBB
        public string ColorTarjeta { get; set; }

        [MaxLength(500)]
        public string RutaImagen { get; set; }

        public bool EstaActiva { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int IdProfesorEncargado { get; set; }
        [ForeignKey("IdProfesorEncargado")]
        public Profesor ProfesorEncargado { get; set; }

        public ICollection<InscripcionActividad> Inscripciones { get; set; }
    }
}
