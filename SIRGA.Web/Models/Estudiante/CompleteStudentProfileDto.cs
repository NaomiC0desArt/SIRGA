using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Estudiante
{
    public class CompleteStudentProfileDto
    {
        [Required(ErrorMessage = "El género es requerido")]
        [Display(Name = "Género")]
        public char Gender { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "La provincia es requerida")]
        [Display(Name = "Provincia")]
        public string Province { get; set; }

        [Required(ErrorMessage = "El sector es requerido")]
        [Display(Name = "Sector")]
        public string Sector { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Phone(ErrorMessage = "Número de teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Condiciones Médicas")]
        public string MedicalConditions { get; set; }

        [Display(Name = "Nota Médica")]
        public string MedicalNote { get; set; }

        [Required(ErrorMessage = "El nombre del contacto de emergencia es requerido")]
        [Display(Name = "Contacto de Emergencia")]
        public string EmergencyContactName { get; set; }

        [Required(ErrorMessage = "El teléfono del contacto de emergencia es requerido")]
        [Phone]
        [Display(Name = "Teléfono de Emergencia")]
        public string EmergencyContactPhone { get; set; }

        [Required(ErrorMessage = "Debe establecer una nueva contraseña")]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }
    }
}
