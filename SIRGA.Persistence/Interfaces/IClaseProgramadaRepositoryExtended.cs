using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Interfaces
{
    public interface IClaseProgramadaRepositoryExtended: IClaseProgramadaRepository
    {
        Task<List<ClaseProgramadaConDetalles>> GetAllWithDetailsAsync();
        Task<ClaseProgramadaConDetalles> GetByIdWithDetailsAsync(int id);
    }
}
