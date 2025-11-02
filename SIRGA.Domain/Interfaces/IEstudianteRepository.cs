using SIRGA.Domain.Entities;


namespace SIRGA.Domain.Interfaces
{
    public interface IEstudianteRepository : IGenericRepository<Estudiante>
    {
        Task<Estudiante> GetByMatriculaAsync(string matricula);
        Task<Estudiante> GetByApplicationUserIdAsync(string applicationUserId);
        Task<string> GetLastMatriculaAsync();
        Task<bool> ExistsByMatriculaAsync(string matricula);
    }
}
