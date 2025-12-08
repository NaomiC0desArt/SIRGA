using System.ComponentModel.DataAnnotations.Schema;

namespace SIRGA.Domain.Entities
{
    public class Profesor
    {
        public int Id { get; set; }
        public string Specialty { get; set; } 
        public string ApplicationUserId { get; set; }
    }
}
