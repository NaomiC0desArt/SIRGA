using SIRGA.Domain.Entities;
using SIRGA.Domain.ReadModels;

namespace SIRGA.Domain.Interfaces
{
    public interface IProfesorRepository : IGenericRepository<Profesor>
    {
        Task<Profesor> GetByApplicationUserIdAsync(string applicationUserId);
        Task<ProfesorConUsuario> GetProfesorConUsuarioAsync(int id);
        Task<Dictionary<int, ProfesorConUsuario>> GetProfesoresConUsuarioAsync(List<int> ids);


    }
}
