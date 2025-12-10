using SIRGA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Interfaces
{
    public interface IPeriodoRepository : IGenericRepository<Periodo>
    {
        Task<List<Periodo>> GetAllWithAnioEscolarAsync();
        Task<Periodo> GetByIdWithAnioEscolarAsync(int id);
        Task<List<Periodo>> GetByAnioEscolarAsync(int anioEscolarId);
        Task<bool> ExistePeriodoAsync(int numero, int anioEscolarId);

        Task<Periodo> GetPeriodoActivoAsync();
        Task<bool> TienePeriodoActivoAsync(int anioEscolarId);
        Task<int> ContarPeriodosPorAnioAsync(int anioEscolarId);
    }
}
