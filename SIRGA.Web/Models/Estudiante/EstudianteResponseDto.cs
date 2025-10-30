namespace SIRGA.Web.Models.Estudiante
{
    public class EstudianteResponseDto
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Matricula { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char Gender { get; set; }
        public string DateOfBirth { get; set; } // Viene como string desde la API
        public string Province { get; set; }
        public string Sector { get; set; }
        public string Address { get; set; }
        public string DateOfEntry { get; set; } // Viene como string desde la API
        public bool IsActive { get; set; }
        public bool MustCompleteProfile { get; set; }
        public string TemporaryPassword { get; set; }
    }
}
