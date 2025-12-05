using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Interfaces
{
    public interface IActividadExtracurricularRepository : IBaseRepository<ActividadExtracurricular>
    {
        Task<List<ActividadExtracurricular>> GetActividadesActivasAsync();
        Task<List<ActividadExtracurricular>> GetActividadesPorCategoriaAsync(string categoria);
        Task<ActividadExtracurricular> GetActividadConDetallesAsync(int id);
        Task<List<Estudiante>> GetEstudiantesInscritosAsync(int idActividad);
    }
}
