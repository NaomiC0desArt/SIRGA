namespace SIRGA.Application.DTOs.ResponseDto
{
    public class GradoResponseDto
    {
        public int Id { get; set; }
        public string GradeName { get; set; }
        public string Section { get; set; }
        public int StudentsLimit { get; set; } = 25;
    }
}
