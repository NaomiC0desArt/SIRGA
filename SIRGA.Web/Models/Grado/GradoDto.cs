using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.Grado
{
    public class GradoDto
    {
        public int Id { get; set; }

        [Display(Name = "Grado")]
        public string GradeName { get; set; }

        public string Nivel { get; set; }
    }
}
