using System.ComponentModel.DataAnnotations;

namespace SIRGA.Domain.Entities
{
    public class AnioEscolar
    {
        [Key]
        public int Id { get; set; }
        public int AnioInicio { get; set; }
        public int AnioFin { get; set; }
        public bool Activo { get; set; }
    }
}
