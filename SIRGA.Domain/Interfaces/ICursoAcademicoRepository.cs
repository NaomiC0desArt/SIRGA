using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;

namespace SIRGA.Domain.Interfaces
{
    public interface ICursoAcademicoRepository : IGenericRepository<CursoAcademico>
    {

        Task<List<CursoAcademico>> GetAllWithDetailsAsync();
        Task<CursoAcademico> GetByIdWithDetailsAsync(int id);
        Task<List<CursoAcademico>> GetByAnioEscolarAsync(int idAnioEscolar);
        Task<int> GetCantidadEstudiantesEnSeccionAsync(int idSeccion, int idAnioEscolar);
        Task<bool> ExisteCursoAsync(int idGrado, int idSeccion, int idAnioEscolar, int? excludeId = null);
    }
}
