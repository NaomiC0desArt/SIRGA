using SIRGA.Web.Models.Grado;
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.CursoAcademico
{
    public class CursoAcademicoDto
    {
        public int Id { get; set; }

        [Display(Name = "Grado")]
        public int IdGrado { get; set; }

        public GradoDto Grado { get; set; }

        [Display(Name = "Año Escolar")]
        public string SchoolYear { get; set; }
    }
}
