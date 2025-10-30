namespace SIRGA.Web.Models.Profesor
{
    public class ProfesorResponseDto
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Province { get; set; }
        public string Sector { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public string DateOfEntry { get; set; }
        public bool IsActive { get; set; }
        public bool MustCompleteProfile { get; set; }
        public string TemporaryPassword { get; set; }
    }
}
