using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class CursoAcademico
    {
        public int Id { get; set; }

        
        public int IdGrado { get; set; }
        public int IdSeccion { get; set; }
        public int IdAnioEscolar { get; set; }
        public int? IdAulaBase { get; set; }  

        // Navigation Properties
        [ForeignKey("IdGrado")]
        public Grado Grado { get; set; }

        [ForeignKey("IdSeccion")]
        public Seccion Seccion { get; set; }

        [ForeignKey("IdAnioEscolar")]
        public AnioEscolar AnioEscolar { get; set; }

        [ForeignKey("IdAulaBase")]
        public Aula AulaBase { get; set; }

        

        [NotMapped]
        public string NombreCompleto =>
            $"{Grado?.GradeName} - Sección {Seccion?.Nombre} ({AnioEscolar?.Periodo})";
    }

}
