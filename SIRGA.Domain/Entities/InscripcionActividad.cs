using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class InscripcionActividad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdEstudiante { get; set; }
        [ForeignKey("IdEstudiante")]
        public Estudiante Estudiante { get; set; }

        public int IdActividad { get; set; }
        [ForeignKey("IdActividad")]
        public ActividadExtracurricular Actividad { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        public bool EstaActiva { get; set; } = true;
    }
}
