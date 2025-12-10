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
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<AnioEscolar> AniosEscolares { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<Aula> Aulas { get; set; }

        public DbSet<Asignatura> Asignaturas { get; set; }
		public DbSet<CursoAcademico> CursosAcademicos { get; set; }
		public DbSet<ClaseProgramada> ClasesProgramadas { get; set; }
		public DbSet<Inscripcion> Inscripciones { get; set; }

        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<ActividadExtracurricular> ActividadesExtracurriculares { get; set; }
        public DbSet<InscripcionActividad> InscripcionesActividades { get; set; }

        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<CalificacionDetalle> CalificacionDetalles { get; set; }
        public DbSet<ComponenteCalificacion> ComponentesCalificacion { get; set; }
        public DbSet<HistorialCalificacion> HistorialCalificaciones { get; set; }

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

            builder.Entity<Grado>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.GradeName).IsRequired().HasMaxLength(50);
            });

            // Configuración de Sección
            builder.Entity<Seccion>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(10);
                entity.HasIndex(s => s.Nombre).IsUnique();
            });

            // Configuración de Año Escolar
            builder.Entity<AnioEscolar>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AnioInicio).IsRequired();
                entity.Property(a => a.AnioFin).IsRequired();
            });

            // Configuración de Periodo
            builder.Entity<Periodo>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Numero).IsRequired();
                entity.HasOne(p => p.AnioEscolar)
                    .WithMany()
                    .HasForeignKey(p => p.AnioEscolarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Curso Académico
            builder.Entity<CursoAcademico>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Grado)
                    .WithMany()
                    .HasForeignKey(c => c.IdGrado)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Seccion)
                    .WithMany()
                    .HasForeignKey(c => c.IdSeccion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.AnioEscolar)
                    .WithMany()
                    .HasForeignKey(c => c.IdAnioEscolar)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.AulaBase)
                    .WithMany()
                    .HasForeignKey(c => c.IdAulaBase)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Índice único para evitar duplicados
                entity.HasIndex(c => new { c.IdGrado, c.IdSeccion, c.IdAnioEscolar })
                    .IsUnique()
                    .HasDatabaseName("IX_CursoAcademico_Unique");
            });

            // Configuración de Clase Programada
            builder.Entity<ClaseProgramada>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Asignatura)
                    .WithMany()
                    .HasForeignKey(c => c.IdAsignatura)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Profesor)
                    .WithMany()
                    .HasForeignKey(c => c.IdProfesor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CursoAcademico)
                    .WithMany()
                    .HasForeignKey(c => c.IdCursoAcademico)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Inscripción
            builder.Entity<Inscripcion>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.HasOne(i => i.Estudiante)
                    .WithMany()
                    .HasForeignKey(i => i.IdEstudiante)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.CursoAcademico)
                    .WithMany()
                    .HasForeignKey(i => i.IdCursoAcademico)
                    .OnDelete(DeleteBehavior.Restrict);

                // Un estudiante solo puede estar inscrito una vez en un curso
                entity.HasIndex(i => new { i.IdEstudiante, i.IdCursoAcademico })
                    .IsUnique()
                    .HasDatabaseName("IX_Inscripcion_Unique");
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

            builder.Entity<Calificacion>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Estudiante)
                    .WithMany()
                    .HasForeignKey(c => c.IdEstudiante)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Asignatura)
                    .WithMany()
                    .HasForeignKey(c => c.IdAsignatura)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CursoAcademico)
                    .WithMany()
                    .HasForeignKey(c => c.IdCursoAcademico)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Periodo)
                    .WithMany()
                    .HasForeignKey(c => c.IdPeriodo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Profesor)
                    .WithMany()
                    .HasForeignKey(c => c.IdProfesor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Detalles)
                    .WithOne(d => d.Calificacion)
                    .HasForeignKey(d => d.IdCalificacion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => new { c.IdEstudiante, c.IdAsignatura, c.IdPeriodo })
                    .IsUnique()
                    .HasDatabaseName("IX_Calificacion_Unique");

                entity.HasIndex(c => c.IdProfesor)
                    .HasDatabaseName("IX_Calificacion_Profesor");

                entity.HasIndex(c => new { c.IdCursoAcademico, c.IdAsignatura, c.IdPeriodo })
                    .HasDatabaseName("IX_Calificacion_Curso_Asignatura_Periodo");

                entity.HasIndex(c => c.Publicada)
                    .HasDatabaseName("IX_Calificacion_Publicada");
            });

            builder.Entity<CalificacionDetalle>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasOne(d => d.Calificacion)
                    .WithMany(c => c.Detalles)
                    .HasForeignKey(d => d.IdCalificacion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Componente)
                    .WithMany()
                    .HasForeignKey(d => d.IdComponenteCalificacion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(d => new { d.IdCalificacion, d.IdComponenteCalificacion })
                    .IsUnique()
                    .HasDatabaseName("IX_CalificacionDetalle_Unique");

                entity.HasIndex(d => d.IdCalificacion)
                    .HasDatabaseName("IX_CalificacionDetalle_Calificacion");
            });

            builder.Entity<ComponenteCalificacion>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasIndex(c => new { c.TipoAsignatura, c.Nombre })
                    .HasDatabaseName("IX_ComponenteCalificacion_Tipo_Nombre");

                entity.HasIndex(c => new { c.TipoAsignatura, c.Orden })
                    .HasDatabaseName("IX_ComponenteCalificacion_Tipo_Orden");

                entity.HasIndex(c => c.Activo)
                    .HasDatabaseName("IX_ComponenteCalificacion_Activo");
            });

            builder.Entity<HistorialCalificacion>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.HasOne(h => h.Calificacion)
                    .WithMany()
                    .HasForeignKey(h => h.IdCalificacion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(h => h.IdCalificacion)
                    .HasDatabaseName("IX_HistorialCalificacion_Calificacion");

                entity.HasIndex(h => h.FechaModificacion)
                    .HasDatabaseName("IX_HistorialCalificacion_Fecha");
            });
            // Para que en caso de que borremos un grado noborre los CursosAcademicos.
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}

	}
}
