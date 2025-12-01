using SIRGA.Domain.Enum;

namespace SIRGA.Application.DTOs.Entities
{
    public class AsignaturaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public TipoAsignatura TipoAsignatura { get; set; }
    }
}
