using SIRGA.Domain.Entities;

namespace SIRGA.Application.DTOs.ResponseDto
{
    public class CursoAcademicoResponseDto
    {
        public int Id { get; set; }
        public int IdGrado { get; set; }
        public Grado Grado { get; set; }
        public string SchoolYear { get; set; } //2023-2024
    }
}
