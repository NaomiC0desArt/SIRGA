using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Grado
{
    public class PeriodoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de periodo es obligatorio")]
        [Range(1, 4, ErrorMessage = "El periodo debe estar entre 1 y 4")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El año escolar es obligatorio")]
        public int AnioEscolarId { get; set; }

        public string AnioEscolar { get; set; }
    }

}
