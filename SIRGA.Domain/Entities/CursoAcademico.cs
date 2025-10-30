using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class CursoAcademico
    {
        public int Id { get; set; }
        
        public int IdGrado { get; set; }
        [ForeignKey("IdGrado")]
        public Grado Grado { get; set; }
        public string SchoolYear { get; set; } //2023-2024
    }
}
