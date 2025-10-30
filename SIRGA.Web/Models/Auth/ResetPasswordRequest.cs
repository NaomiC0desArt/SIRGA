using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
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
