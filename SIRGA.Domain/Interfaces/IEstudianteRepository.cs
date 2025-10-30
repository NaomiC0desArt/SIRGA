using SIRGA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Interfaces
{
    public interface IEstudianteRepository
    {
        Task<Estudiante> AddAsync(Estudiante estudiante);
        Task<Estudiante> GetByIdAsync(int id);
        Task<Estudiante> GetByMatriculaAsync(string matricula);
        Task<Estudiante> GetByApplicationUserIdAsync(string applicationUserId);
        Task<List<Estudiante>> GetAllAsync();
        Task<Estudiante> UpdateAsync(Estudiante estudiante);
        Task<bool> DeleteAsync(int id);
        Task<string> GetLastMatriculaAsync();
        Task<bool> ExistsByMatriculaAsync(string matricula);
    }
}
