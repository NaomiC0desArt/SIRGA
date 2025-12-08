using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;

namespace SIRGA.Domain.Interfaces
{
    public interface IAsignaturaRepository : IGenericRepository<Asignatura>
    {
        Task<int> GetProfesoresCountAsync(int asignaturaId);
    }
}
