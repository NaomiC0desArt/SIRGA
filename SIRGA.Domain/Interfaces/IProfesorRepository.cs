using SIRGA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Domain.Interfaces
{
    public interface IProfesorRepository
    {
        Task<Profesor> AddAsync(Profesor profesor);
        Task<Profesor> GetByIdAsync(int id);
        Task<Profesor> GetByApplicationUserIdAsync(string applicationUserId);
        Task<List<Profesor>> GetAllAsync();
        Task<Profesor> UpdateAsync(Profesor profesor);
        Task<bool> DeleteAsync(int id);
    }
}
