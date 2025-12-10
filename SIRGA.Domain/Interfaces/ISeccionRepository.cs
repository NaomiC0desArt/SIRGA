using SIRGA.Domain.Entities;


namespace SIRGA.Domain.Interfaces
{
    public interface ISeccionRepository : IGenericRepository<Seccion>
    {
        Task<Seccion> GetByNombreAsync(string nombre);
        Task<bool> ExisteSeccionAsync(string nombre);
        Task<string> GetProximaLetraDisponibleAsync();
    }
}
