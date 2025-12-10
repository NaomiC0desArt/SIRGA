using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class Calificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdEstudiante { get; set; }
        [ForeignKey("IdEstudiante")]
        public Estudiante Estudiante { get; set; }

        [Required]
        public int IdAsignatura { get; set; }
        [ForeignKey("IdAsignatura")]
        public Asignatura Asignatura { get; set; }

        [Required]
        public int IdCursoAcademico { get; set; }
        [ForeignKey("IdCursoAcademico")]
        public CursoAcademico CursoAcademico { get; set; }

        [Required]
        public int IdPeriodo { get; set; }
        [ForeignKey("IdPeriodo")]
        public Periodo Periodo { get; set; }

        [Required]
        public int IdProfesor { get; set; }
        [ForeignKey("IdProfesor")]
        public Profesor Profesor { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Total { get; set; }

        public bool Publicada { get; set; } = false;

        public DateTime? FechaPublicacion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimaModificacion { get; set; }

        [MaxLength(500)]
        public string Observaciones { get; set; }

        // Navegación
        public ICollection<CalificacionDetalle> Detalles { get; set; }
    }
}
