namespace SIRGA.Web.Models.Auth
{
    public class LoginResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool MustCompleteProfile { get; set; }
        public string JWToken { get; set; } = string.Empty;
    }
}
