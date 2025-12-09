using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities.Asigantura
{
    public class AsignaturaDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "GFavor agregar una descripcion valida")]
        [MaxLength(125, ErrorMessage = "La descripción no puede exceder los 125 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de asignatura es requerido")]
        public string TipoAsignatura { get; set; }
    }

}
