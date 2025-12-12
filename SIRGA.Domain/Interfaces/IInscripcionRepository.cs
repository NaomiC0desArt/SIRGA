using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces.Base;
using SIRGA.Domain.ReadModels;
using System.Linq.Expressions;

namespace SIRGA.Domain.Interfaces
{
    public interface IInscripcionRepository : IGenericRepository<Inscripcion>
    {
        Task<Inscripcion> GetByConditionAsync(Expression<Func<Inscripcion, bool>> expression);
        Task<List<Inscripcion>> GetAllByConditionAsync(Expression<Func<Inscripcion, bool>> expression);

        // métodos con datos enriquecidos
        Task<InscripcionConDetalles> GetInscripcionConDetallesAsync(int id);
        Task<List<InscripcionConDetalles>> GetAllInscripcionesConDetallesAsync();
        Task<List<InscripcionConDetalles>> GetInscripcionesPorCursoAsync(int idCursoAcademico);

        Task<Inscripcion> GetInscripcionActivaByEstudianteIdAsync(int estudianteId);
    }
}
