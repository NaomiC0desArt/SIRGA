

using SIRGA.Domain.Entities;

namespace SIRGA.Domain.Interfaces
{
    public interface IAsistenciaRepository : IGenericRepository<Asistencia>
    {
        Task<List<Asistencia>> GetAsistenciasByClaseAndFechaAsync(int idClaseProgramada, DateTime fecha);
        Task<Asistencia?> GetAsistenciaByEstudianteClaseFechaAsync(int idEstudiante, int idClaseProgramada, DateTime fecha);
        Task<List<Asistencia>> GetAsistenciasByEstudianteAsync(int idEstudiante, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<List<Asistencia>> GetAsistenciasByProfesorAsync(int idProfesor, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<List<Asistencia>> GetAsistenciasRequierenJustificacionAsync();
        Task<bool> ExisteAsistenciaAsync(int idEstudiante, int idClaseProgramada, DateTime fecha);
        Task<int> GetTotalAsistenciasByEstudianteAsync(int idEstudiante, string? estado = null);
        Task<List<Asistencia>> GetHistorialAsistenciasAsync(int idClaseProgramada, DateTime fechaInicio, DateTime fechaFin);
    }
}
