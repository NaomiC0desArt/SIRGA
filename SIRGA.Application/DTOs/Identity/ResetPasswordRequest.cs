using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Identity
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

    }
}
