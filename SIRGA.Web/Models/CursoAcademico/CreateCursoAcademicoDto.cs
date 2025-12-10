using System.ComponentModel.DataAnnotations;

namespace SIRGA.Web.Models.CursoAcademico
{
    public class CreateCursoAcademicoDto
    {
        [Required(ErrorMessage = "Favor seleccionar un grado")]
        public int IdGrado { get; set; }

        [Required(ErrorMessage = "La sección es obligatoria")]
        public int IdSeccion { get; set; }

        [Required(ErrorMessage = "El año escolar es obligatorio")]
        public int IdAnioEscolar { get; set; }

        public int? IdAulaBase { get; set; }
    }
}
