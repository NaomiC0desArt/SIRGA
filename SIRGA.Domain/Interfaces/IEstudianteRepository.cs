using SIRGA.Domain.Entities;
using SIRGA.Domain.ReadModels;


namespace SIRGA.Domain.Interfaces
{
    public interface IEstudianteRepository : IGenericRepository<Estudiante>
    {
        Task<Estudiante> GetByMatriculaAsync(string matricula);
        Task<Estudiante> GetByApplicationUserIdAsync(string applicationUserId);
        Task<string> GetLastMatriculaAsync();
        Task<bool> ExistsByMatriculaAsync(string matricula);
        Task<EstudianteConUsuario> GetEstudianteConUsuarioAsync(int id);
        Task<List<EstudianteConUsuario>> GetEstudiantesConUsuarioAsync(List<int> ids);
    }
}
