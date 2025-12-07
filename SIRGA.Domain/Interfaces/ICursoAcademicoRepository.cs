using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;

namespace SIRGA.Domain.Interfaces
{
    public interface ICursoAcademicoRepository : IGenericRepository<CursoAcademico>
    {
        Task<List<CursoAcademico>> GetAllWithGradoAsync();
        Task<CursoAcademico?> GetByIdWithGradoAsync(int id);
    }
}
