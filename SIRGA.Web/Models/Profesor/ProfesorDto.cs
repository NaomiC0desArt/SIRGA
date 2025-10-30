namespace SIRGA.Web.Models.Profesor
{
    public class ProfesorDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public char? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Province { get; set; }
        public string Sector { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public bool IsActive { get; set; }
        public DateOnly DateOfEntry { get; set; }
        public bool MustCompleteProfile { get; set; }
    }
}
