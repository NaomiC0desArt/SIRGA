using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class Grado
    {
        public int Id { get; set; }
        public string GradeName { get; set; }
        public string Section { get; set; }
        public int StudentsLimit { get; set; } = 25;
    }
}
