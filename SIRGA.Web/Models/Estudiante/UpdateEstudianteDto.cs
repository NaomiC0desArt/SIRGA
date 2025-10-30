using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Estudiante
{
    public class UpdateEstudianteDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Provincia")]
        public string Province { get; set; }

        [StringLength(100)]
        [Display(Name = "Sector")]
        public string Sector { get; set; }

        [StringLength(250)]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; }
    }
}
