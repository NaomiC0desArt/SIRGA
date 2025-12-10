using SIRGA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Interfaces
{
    public interface ICalificacionRepository : IGenericRepository<Calificacion>
    {
        Task<Calificacion> GetCalificacionConDetallesAsync(int idEstudiante, int idAsignatura, int idPeriodo);
        Task<List<Calificacion>> GetCalificacionesPorProfesorYPeriodoAsync(int idProfesor, int idPeriodo);
        Task<List<Calificacion>> GetCalificacionesPorEstudianteAsync(int idEstudiante);
        Task<List<Calificacion>> GetCalificacionesPorCursoYAsignaturaAsync(int idCurso, int idAsignatura, int idPeriodo);
        Task<bool> ExisteCalificacionAsync(int idEstudiante, int idAsignatura, int idPeriodo);
        Task<int> ContarCalificacionesPublicadasAsync(int idProfesor, int idPeriodo);
        Task<int> ContarCalificacionesPendientesAsync(int idProfesor, int idPeriodo);
    }
}
