namespace SIRGA.Application.DTOs.Entities
{
    public class GradoDto
    {
        public int Id { get; set; }
        public string GradeName { get; set; }
        public string Section { get; set; }
        public int StudentsLimit { get; set; } = 25;
    }
}
