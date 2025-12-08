using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.ActividadExtracurricular
{
    public class UpdateActividadViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public string Categoria { get; set; }

        [MaxLength(300)]
        public string Requisitos { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Required]
        public string DiaSemana { get; set; }

        [MaxLength(100)]
        public string Ubicacion { get; set; }

        [Required]
        public string ColorTarjeta { get; set; }

        [Required]
        public int IdProfesorEncargado { get; set; }

        public bool EstaActiva { get; set; }

        public IFormFile Imagen { get; set; }

        public string RutaImagenActual { get; set; }
    }
}
