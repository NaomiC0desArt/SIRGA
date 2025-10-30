using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.UserManagement.Profesor
{
    public class CompleteTeacherProfileDto
    {
        [Required(ErrorMessage = "El género es requerido")]
        [RegularExpression("^[MFmf]$", ErrorMessage = "El género debe ser M o F")]
        public char Gender { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "La provincia es requerida")]
        [StringLength(100)]
        public string Province { get; set; }

        [Required(ErrorMessage = "El sector es requerido")]
        [StringLength(100)]
        public string Sector { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250)]
        public string Address { get; set; }

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Specialty { get; set; }

        // Contraseña nueva
        [Required(ErrorMessage = "Debe establecer una nueva contraseña")]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
