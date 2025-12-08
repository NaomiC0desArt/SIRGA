using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class PeriodoRepository : GenericRepository<Periodo>, IPeriodoRepository
    {
        public PeriodoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
