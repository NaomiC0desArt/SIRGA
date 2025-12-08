using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
using SIRGA.Domain.ReadModels;
using System.Threading.Tasks;

namespace SIRGA.Domain.Interfaces
{
    public interface IClaseProgramadaRepository : IGenericRepository<ClaseProgramada>
    {
        Task<List<ClaseProgramadaConDetalles>> GetAllWithDetailsAsync();
        Task<ClaseProgramadaConDetalles> GetByIdWithDetailsAsync(int id);
        Task<List<ClaseProgramada>> GetClasesByProfesorAndDayAsync(int idProfesor, DayOfWeek diaSemana);
        Task<List<ClaseConDetallesParaHorario>> GetClasesPorCursoAcademicoAsync(int idCursoAcademico);
        Task<List<ClaseConDetallesParaHorario>> GetClasesPorCursoYDiaAsync(int idCursoAcademico, DayOfWeek dia);
    }
}
