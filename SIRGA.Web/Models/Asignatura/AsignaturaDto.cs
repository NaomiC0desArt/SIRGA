using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asignatura
{
    public class AsignaturaDto
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TipoAsignatura { get; set; }
    }
}
