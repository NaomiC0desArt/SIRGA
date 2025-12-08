using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class CursoAcademico
    {
        public int Id { get; set; }
        
        public int IdGrado { get; set; }
        [ForeignKey("IdGrado")]
        public Grado Grado { get; set; }
        public string SchoolYear { get; set; }
    }
}
