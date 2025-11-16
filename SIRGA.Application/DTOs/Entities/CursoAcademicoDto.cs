using SIRGA.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Application.DTOs.Entities
{
    public class CursoAcademicoDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El ID del grado es requerido")]
        public int IdGrado { get; set; }
        [Required(ErrorMessage = "El año escolar es requerido")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "El formato debe ser YYYY-YYYY (ej: 2024-2025)")]
        public string SchoolYear { get; set; } //2023-2024
    }
}
