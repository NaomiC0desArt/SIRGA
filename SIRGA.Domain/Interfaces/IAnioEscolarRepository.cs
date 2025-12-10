using SIRGA.Domain.Entities;


namespace SIRGA.Domain.Interfaces
{
    public interface IAnioEscolarRepository : IGenericRepository<AnioEscolar>
    {
        Task<AnioEscolar> GetAnioActivoAsync();
        Task<bool> ExisteAnioActivoAsync();
        Task DesactivarTodosAsync();
    }
}
