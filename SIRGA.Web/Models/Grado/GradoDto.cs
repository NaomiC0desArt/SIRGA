using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Grado
{
    public class GradoDto
    {
        public int Id { get; set; }

        [Display(Name = "Grado")]
        public string GradeName { get; set; }

        [Display(Name = "Sección")]
        public string Section { get; set; }

        [Display(Name = "Límite de Estudiantes")]
        public int StudentsLimit { get; set; }
    }
}
