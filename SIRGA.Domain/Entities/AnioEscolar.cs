using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class AnioEscolar
        {
            [Key]
            public int Id { get; set; }
            public int AnioInicio { get; set; }
            public int AnioFin { get; set; }
            public bool Activo { get; set; }

        [NotMapped]
        public string? Periodo => $"{AnioInicio}-{AnioFin}";
    }
    
}
