

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SIRGA.Identity.Interfaces;
using SIRGA.Identity.Seeds;
using SIRGA.Identity.Services;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.DbContext;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace SIRGA.Identity.Register
{
	public static class IdentityDependencies
	{
		public static void AddIdentityLayer(this IServiceCollection services, IConfiguration configuration)
		{

			var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			// configuracion del DbContext para Identity
			services.AddDbContext<ApplicationDbContext> (options =>
			{
			options.EnableSensitiveDataLogging();
				options.UseSqlServer(connectionString,
					m => m.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

			});

            // configuración de Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configuración de contraseñas
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                // Configuración de usuario
                options.User.RequireUniqueEmail = true;

                // Configuración de confirmación
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

            // Configuración de tokens de confirmación de email
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(24); // Cambiado de 5 minutos a 24 horas
            });

            // CONFIGURACIÓN DE JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // REGISTRAR SERVICIO JWT
            services.AddScoped<IJwtService, JwtService>();
        }

		public static async Task RunIdentitySeeds(this IServiceProvider serviceProvider)
		{
            Console.WriteLine("🌱 RunIdentitySeeds iniciado...");

            using (var scope = serviceProvider.CreateScope())
			{
				var service = scope.ServiceProvider;

				var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
				var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
				var loggerFactory = service.GetRequiredService<ILoggerFactory>();
				var logger = loggerFactory.CreateLogger("IdentitySeedRunner");

				try
				{
					logger.LogInformation("Iniciando la siembra de datos de Identity.");

					// 1. Ejecutar la siembra de Roles
					await DefaultRoles.SeedAsync(roleManager);
					logger.LogInformation("Roles sembrados.");

					// 2. Ejecutar la siembra del usuario SuperAdmin
					await AdminUser.SeedAsync(userManager, roleManager);
					logger.LogInformation("Usuario Admin sembrado.");

					// 3. Ejecutar otros seeds 

					logger.LogInformation("Siembra de Identity completada exitosamente.");
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Ocurrió un error crítico durante la siembra de datos de Identity.");
					
				}
			}
		}
	}
	
}

