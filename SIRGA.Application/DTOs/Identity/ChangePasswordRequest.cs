using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Identity
{
    public class ChangePasswordRequest
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}
