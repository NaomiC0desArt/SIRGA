using SIRGA.Domain.Entities;

namespace SIRGA.Domain.Interfaces
{
    public interface IProfesorRepository : IGenericRepository<Profesor>
    {
        Task<Profesor> GetByApplicationUserIdAsync(string applicationUserId);
        
    }
}
