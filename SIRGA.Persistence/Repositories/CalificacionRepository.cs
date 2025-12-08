using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class CalificacionRepository : GenericRepository<Calificacion>, ICalificacionRepository
    {
        public CalificacionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
