using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class CalificacionDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdCalificacion { get; set; }
        [ForeignKey("IdCalificacion")]
        public Calificacion Calificacion { get; set; }

        [Required]
        public int IdComponenteCalificacion { get; set; }
        [ForeignKey("IdComponenteCalificacion")]
        public ComponenteCalificacion Componente { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Valor { get; set; }
    }
}
