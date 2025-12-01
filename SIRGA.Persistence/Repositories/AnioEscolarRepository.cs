using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class AnioEscolarRepository : GenericRepository<AnioEscolar>, IAnioEscolarRepository
    {
        public AnioEscolarRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
