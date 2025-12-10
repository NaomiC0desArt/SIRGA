using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class ComponenteCalificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoAsignatura { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal ValorMaximo { get; set; }

        [Required]
        public int Orden { get; set; }

        public bool Activo { get; set; } = true;
    }
}
