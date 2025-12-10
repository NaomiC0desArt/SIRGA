using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;
using SIRGA.Persistence.DbContext;
using SIRGA.Persistence.Repositories.Base;

namespace SIRGA.Persistence.Repositories
{
    public class SeccionRepository : GenericRepository<Seccion>, ISeccionRepository
    {
        public SeccionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Seccion> GetByNombreAsync(string nombre)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Nombre == nombre);
        }

        public async Task<bool> ExisteSeccionAsync(string nombre)
        {
            return await _dbSet.AnyAsync(s => s.Nombre == nombre);
        }

        public async Task<string> GetProximaLetraDisponibleAsync()
        {
            var seccionesExistentes = await _dbSet
            .Select(s => s.Nombre)
            .ToListAsync();

            var letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            foreach (var letra in letras)
            {
                if (!seccionesExistentes.Contains(letra.ToString()))
                {
                    return letra.ToString();
                }
            }

            for (int i = 0; i < letras.Length; i++)
            {
                for (int j = 0; j < letras.Length; j++)
                {
                    string nombre = $"{letras[i]}{letras[j]}";
                    if (!seccionesExistentes.Contains(nombre))
                    {
                        return nombre;
                    }
                }
            }

            return $"S{new Random().Next(1, 100)}";
        }
    }
}
