using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
using SIRGA.Domain.ReadModels;

namespace SIRGA.Domain.Interfaces
{
    public interface IActividadExtracurricularRepository : IGenericRepository<ActividadExtracurricular>
    {
       
        Task<ActividadConDetalles> GetActividadDetalladaAsync(int id);
        Task<List<ActividadConDetalles>> GetAllActividadesDetalladasAsync();
        Task<List<ActividadConDetalles>> GetActividadesActivasDetalladasAsync();
        Task<List<ActividadConDetalles>> GetActividadesPorCategoriaDetalladasAsync(string categoria);

        
        Task<ActividadExtracurricular> GetActividadConDetallesAsync(int id);
        Task<List<ActividadExtracurricular>> GetActividadesActivasAsync();
        Task<List<ActividadExtracurricular>> GetActividadesPorCategoriaAsync(string categoria);
    }
}
