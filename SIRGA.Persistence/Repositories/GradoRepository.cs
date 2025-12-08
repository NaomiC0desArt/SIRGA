using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class GradoRepository : GenericRepository<Grado>, IGradoRepository {
        public GradoRepository(ApplicationDbContext context) : base(context) { }
    }
}
