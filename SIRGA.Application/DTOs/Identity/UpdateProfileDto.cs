using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.DTOs.Identity
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "La provincia es requerida")]
        [StringLength(100)]
        public string Province { get; set; }

        [Required(ErrorMessage = "El sector es requerido")]
        [StringLength(100)]
        public string Sector { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250)]
        public string Address { get; set; }

        public string Photo { get; set; }

        
        [Required(ErrorMessage = "Debe ingresar su contraseña actual")]
        public string CurrentPassword { get; set; }
    }

    public class BasicUserInfoDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
    }
}
