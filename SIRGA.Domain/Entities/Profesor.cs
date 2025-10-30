using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Entities
{
    public class Profesor
    {
        public int Id { get; set; }
        public string Specialty { get; set; } //idk if we are going to create a specialty table
        public string ApplicationUserId { get; set; }
    }
}
