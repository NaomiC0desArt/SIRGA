namespace SIRGA.Web.Models.Auth
{
    public class ApiAuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LoginResponse Data { get; set; }
        public List<string> Errors { get; set; }
    }
}
