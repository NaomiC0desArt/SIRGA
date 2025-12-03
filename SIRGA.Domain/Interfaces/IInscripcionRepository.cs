using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
using System.Linq.Expressions;

namespace SIRGA.Domain.Interfaces
{
    public interface IInscripcionRepository : IBaseRepository<Inscripcion>
    {
        Task<List<Inscripcion>> GetAllByConditionAsync(Expression<Func<Inscripcion, bool>> predicate);
        Task<Inscripcion> GetByConditionAsync(Expression<Func<Inscripcion, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<Inscripcion, bool>> predicate);
    }
}
