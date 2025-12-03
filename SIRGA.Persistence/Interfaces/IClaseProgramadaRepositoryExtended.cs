using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Interfaces
{
    public interface IClaseProgramadaRepositoryExtended: IClaseProgramadaRepository
    {
        Task<List<ClaseProgramadaConDetalles>> GetAllWithDetailsAsync();
        Task<ClaseProgramadaConDetalles> GetByIdWithDetailsAsync(int id);
        Task<List<ClaseProgramada>> GetClasesByProfesorAndDayAsync(int idProfesor, DayOfWeek diaSemana);
        Task<List<ClaseProgramada>> GetAllByConditionAsync(Expression<Func<ClaseProgramada, bool>> predicate);
        Task<ClaseProgramada> GetByConditionAsync(Expression<Func<ClaseProgramada, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<ClaseProgramada, bool>> predicate);
    }
}
