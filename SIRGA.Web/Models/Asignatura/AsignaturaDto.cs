using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Asignatura
{
    public class AsignaturaDto
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Código")]
        public string? Codigo { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Tipo de Asignatura")]
        public string TipoAsignatura { get; set; }
    }
}
