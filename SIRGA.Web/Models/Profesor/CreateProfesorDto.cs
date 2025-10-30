using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Profesor
{
    public class CreateProfesorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }
        [StringLength(200)]
        [Display(Name = "Especialidad")]
        public string Specialty { get; set; }
    }
}
