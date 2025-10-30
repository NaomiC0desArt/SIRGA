namespace SIRGA.Web.Models.Profesor
{
    public class UpdateProfesorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public string Sector { get; set; }
        public string Address { get; set; }
        public string Specialty { get; set; }
        public bool IsActive { get; set; }
    }
}
