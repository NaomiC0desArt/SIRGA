using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SIRGA.Domain.Enum;

namespace SIRGA.Domain.Entities
{
    public class Asignatura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string Nombre { get; set; }
        [Required]
        [MaxLength(125, ErrorMessage = "La descripción no puede exceder los 125 caracteres")]
        public string Descripcion { get; set; }
        public string TipoAsignatura { get; set; }
    }
}
