using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Domain.Entities
{
    public class Asignatura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20)]
        public string? Codigo { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(125, ErrorMessage = "La descripción no puede exceder los 125 caracteres")]
        public string Descripcion { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoAsignatura { get; set; }
    }
}
