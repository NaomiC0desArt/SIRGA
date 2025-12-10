using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Periodo
{
    public class PeriodoDto
    {
        [Required(ErrorMessage = "El número de período es obligatorio")]
        [Range(1, 4, ErrorMessage = "El número de período debe ser entre 1 y 4")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "Favor ingresar la fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Favor ingresar la fecha de inicio")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "Seleccione el año escolar")]
        public int AnioEscolarId { get; set; }
    }

}
