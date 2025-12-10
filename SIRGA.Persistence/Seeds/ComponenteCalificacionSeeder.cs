using SIRGA.Domain.Entities;
using SIRGA.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Persistence.Seeds
{
    public static class ComponenteCalificacionSeeder
    {
        public static async Task SeedComponentesAsync(ApplicationDbContext context)
        {
            if (context.ComponentesCalificacion.Any())
            {
                return; // Ya existen componentes
            }

            var componentes = new List<ComponenteCalificacion>
            {
                // ==================== TEÓRICA ====================
                new ComponenteCalificacion
                {
                    Nombre = "Tareas",
                    TipoAsignatura = "Teorica",
                    ValorMaximo = 40,
                    Orden = 1,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Exámenes Teóricos",
                    TipoAsignatura = "Teorica",
                    ValorMaximo = 25,
                    Orden = 2,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Exposiciones",
                    TipoAsignatura = "Teorica",
                    ValorMaximo = 20,
                    Orden = 3,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Participación",
                    TipoAsignatura = "Teorica",
                    ValorMaximo = 15,
                    Orden = 4,
                    Activo = true
                },

                // ==================== PRÁCTICA ====================
                new ComponenteCalificacion
                {
                    Nombre = "Prácticas",
                    TipoAsignatura = "Practica",
                    ValorMaximo = 50,
                    Orden = 1,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Proyecto Final",
                    TipoAsignatura = "Practica",
                    ValorMaximo = 30,
                    Orden = 2,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Teoría",
                    TipoAsignatura = "Practica",
                    ValorMaximo = 10,
                    Orden = 3,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Participación",
                    TipoAsignatura = "Practica",
                    ValorMaximo = 10,
                    Orden = 4,
                    Activo = true
                },

                // ==================== TEÓRICO-PRÁCTICA ====================
                new ComponenteCalificacion
                {
                    Nombre = "Exámenes",
                    TipoAsignatura = "TeoricoPractica",
                    ValorMaximo = 30,
                    Orden = 1,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Prácticas",
                    TipoAsignatura = "TeoricoPractica",
                    ValorMaximo = 40,
                    Orden = 2,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Proyectos",
                    TipoAsignatura = "TeoricoPractica",
                    ValorMaximo = 20,
                    Orden = 3,
                    Activo = true
                },
                new ComponenteCalificacion
                {
                    Nombre = "Participación",
                    TipoAsignatura = "TeoricoPractica",
                    ValorMaximo = 10,
                    Orden = 4,
                    Activo = true
                }
            };

            await context.ComponentesCalificacion.AddRangeAsync(componentes);
            await context.SaveChangesAsync();
        }
    }
}
