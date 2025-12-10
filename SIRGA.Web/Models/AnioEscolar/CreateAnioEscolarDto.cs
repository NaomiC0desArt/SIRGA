using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.AnioEscolar
{
    public class CreateAnioEscolarDto
    {
        [Required(ErrorMessage = "El año de inicio es requerido")]
        [Range(2000, 2100, ErrorMessage = "Año inválido")]
        public int AnioInicio { get; set; }

        [Required(ErrorMessage = "El año de fin es requerido")]
        [Range(2000, 2100, ErrorMessage = "Año inválido")]
        public int AnioFin { get; set; }

        public bool Activo { get; set; } = false;
    }
}
