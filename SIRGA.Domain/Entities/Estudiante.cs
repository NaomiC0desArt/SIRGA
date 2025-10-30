
using System.ComponentModel.DataAnnotations;

namespace SIRGA.Domain.Entities
{
    public class Estudiante
    {
        public int Id { get; set; }
        [Required]
        public string Matricula { get; set; }
        public string MedicalConditions { get; set; }
        public string MedicalNote { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
