﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int IdEstudiante { get; set; }
        [ForeignKey("IdEstudiante")]
        public Estudiante Estudiante { get; set; }
        public int IdCursoAcademico { get; set; }
        [ForeignKey("IdCursoAcademico")]
        public CursoAcademico CursoAcademico { get; set; }
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}
