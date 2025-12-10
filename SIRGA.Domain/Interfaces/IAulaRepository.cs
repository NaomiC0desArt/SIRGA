using SIRGA.Domain.Entities;


namespace SIRGA.Domain.Interfaces
{
    public interface IAulaRepository : IGenericRepository<Aula>
    {
        Task<List<Aula>> GetAulasDisponiblesAsync();
        Task<Aula> GetByCodigoAsync(string codigo);
    }
}
