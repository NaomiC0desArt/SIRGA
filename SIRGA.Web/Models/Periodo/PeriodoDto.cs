using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Periodo
{
    public class PeriodoDto
    {
        [Required(ErrorMessage = "El número de periodo es requerido")]
        [Range(1, 4, ErrorMessage = "El periodo debe estar entre 1 y 4")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El año escolar es requerido")]
        public int AnioEscolarId { get; set; }
    }
}
