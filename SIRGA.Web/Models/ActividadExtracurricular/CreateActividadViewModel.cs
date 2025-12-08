using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.ActividadExtracurricular
{
    public class CreateActividadViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public string Categoria { get; set; }

        [MaxLength(300)]
        public string Requisitos { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida")]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El día de la semana es requerido")]
        public string DiaSemana { get; set; }

        [MaxLength(100)]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "El color de tarjeta es requerido")]
        public string ColorTarjeta { get; set; } = "#3B82F6";

        [Required(ErrorMessage = "El profesor encargado es requerido")]
        public int IdProfesorEncargado { get; set; }

        public IFormFile Imagen { get; set; }
    }
}
