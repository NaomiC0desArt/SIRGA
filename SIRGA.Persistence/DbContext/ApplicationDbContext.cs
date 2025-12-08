using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIRGA.Domain.Entities;
using SIRGA.Identity.Shared.Entities;
using System.Reflection.Emit;

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
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<ActividadExtracurricular> ActividadesExtracurriculares { get; set; }
        public DbSet<InscripcionActividad> InscripcionesActividades { get; set; }

        #region "Modulo de Calificaciones"
        public DbSet<AnioEscolar> AniosEscolares { get; set; }
        //public DbSet<AnnualGrade> AnnualGrates { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            // relacion de estudiantes y profesores con ApplicationUserId

            builder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Matricula)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasIndex(e => e.Matricula)
                    .IsUnique();

                entity.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Estudiante>(e => e.ApplicationUserId)
                .IsRequired();
            });

            builder.Entity<Profesor>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Specialty)
                    .HasMaxLength(200);

                entity.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Profesor>(p => p.ApplicationUserId)
                .IsRequired();
            });

            builder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Fecha)
                    .IsRequired();

                entity.Property(a => a.HoraRegistro)
                    .IsRequired();

                entity.Property(a => a.Estado)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(a => a.Observaciones)
                    .HasMaxLength(500);

                entity.Property(a => a.Justificacion)
                    .HasMaxLength(500);

                entity.HasOne(a => a.Estudiante)
                    .WithMany()
                    .HasForeignKey(a => a.IdEstudiante)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.ClaseProgramada)
                    .WithMany()
                    .HasForeignKey(a => a.IdClaseProgramada)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Profesor)
                    .WithMany()
                    .HasForeignKey(a => a.IdProfesor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(a => new { a.IdEstudiante, a.IdClaseProgramada, a.Fecha })
                    .IsUnique()
                    .HasDatabaseName("IX_Asistencia_Estudiante_Clase_Fecha");

                entity.HasIndex(a => a.Fecha)
                    .HasDatabaseName("IX_Asistencia_Fecha");

                entity.HasIndex(a => a.IdClaseProgramada)
                    .HasDatabaseName("IX_Asistencia_ClaseProgramada");

                entity.HasIndex(a => a.IdEstudiante)
                    .HasDatabaseName("IX_Asistencia_Estudiante");

                entity.HasIndex(a => new { a.RequiereJustificacion, a.Justificacion })
                    .HasDatabaseName("IX_Asistencia_RequiereJustificacion");
            });

            builder.Entity<ActividadExtracurricular>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.ProfesorEncargado)
                    .WithMany()
                    .HasForeignKey(e => e.IdProfesorEncargado)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Inscripciones)
                    .WithOne(i => i.Actividad)
                    .HasForeignKey(i => i.IdActividad)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<InscripcionActividad>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Actividad)
                    .WithMany(a => a.Inscripciones)
                    .HasForeignKey(e => e.IdActividad)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Para que en caso de que borremos un grado noborre los CursosAcademicos.
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}

        //Configurar la precision de los decimales en Calificacion
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(5, 2);
        }


    }
}
