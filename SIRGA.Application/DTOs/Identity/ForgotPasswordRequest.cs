using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Identity
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        public string Origin { get; set; }
    }
}

