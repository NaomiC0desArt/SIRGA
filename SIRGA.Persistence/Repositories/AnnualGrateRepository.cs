using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class AnnualGrateRepository : GenericRepository<AnnualGrade>, IAnnualGrateRepository
    {
        public AnnualGrateRepository(ApplicationDbContext context) : base(context) { }
    }
}
