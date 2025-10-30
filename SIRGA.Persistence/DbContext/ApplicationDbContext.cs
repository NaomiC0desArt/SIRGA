using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Identity.Shared.Entities;

namespace SIRGA.Persistence.DbContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Estudiante> Estudiantes { get; set; }
		public DbSet<Profesor> Profesores { get; set; }
		public DbSet<Grado> Grados { get; set; }
		public DbSet<Asignatura> Asignaturas { get; set; }
		public DbSet<CursoAcademico> CursosAcademicos { get; set; }
		public DbSet<ClaseProgramada> ClasesProgramadas { get; set; }
		public DbSet<Inscripcion> Inscripciones { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            // relacion de estudiantes y profesores con ApplicationUserId"

            // Configuración específica de Estudiante
            builder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Matrícula única y requerida
                entity.Property(e => e.Matricula)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasIndex(e => e.Matricula)
                    .IsUnique();

                // Relación con ApplicationUser
                entity.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Estudiante>(e => e.ApplicationUserId)
                .IsRequired();
            });

            // Configuración específica de Profesor
            builder.Entity<Profesor>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Specialty)
                    .HasMaxLength(200);

                // Relación con ApplicationUser
                entity.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Profesor>(p => p.ApplicationUserId)
                .IsRequired();
            });
            


			// Para que en caso de que borremos un grado noborre los CursosAcademicos.
			foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}

	}
}
