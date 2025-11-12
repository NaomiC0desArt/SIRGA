using SIRGA.Domain.Entities;

namespace SIRGA.Application.DTOs.Entities
{
    public class CursoAcademicoDto
    {
        public int Id { get; set; }
        public int IdGrado { get; set; }
        public Grado Grado { get; set; }
        public string SchoolYear { get; set; } //2023-2024
    }
}
