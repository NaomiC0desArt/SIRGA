using SIRGA.Domain.Enum;

namespace SIRGA.Domain.Entities
{
    public class Grado
    {
        public int Id { get; set; }
        public string GradeName { get; set; }
        public NivelEducativo Nivel { get; set; }
    }
}
