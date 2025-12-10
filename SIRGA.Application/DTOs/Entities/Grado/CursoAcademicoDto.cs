
namespace SIRGA.Application.DTOs.Entities.Grado
{
    public class CursoAcademicoDto
    {
        public int Id { get; set; }
        public int IdGrado { get; set; }
        public int IdSeccion { get; set; }
        public int IdAnioEscolar { get; set; }
        public int? IdAulaBase { get; set; }

        
        public GradoDto Grado { get; set; }
        public SeccionDto Seccion { get; set; }
        public AnioEscolarDto AnioEscolar { get; set; }
        public AulaDto AulaBase { get; set; }
        public string NombreCompleto { get; set; }
    }
}
