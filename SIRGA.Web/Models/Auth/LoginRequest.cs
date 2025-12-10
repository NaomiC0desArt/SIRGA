using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Favor ingresar su correo institucional")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Favor ingresar su contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
