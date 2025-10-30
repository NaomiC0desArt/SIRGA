using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIRGA.Application.Interfaces.Services;
using SIRGA.Application.Interfaces.Services.Email;
using SIRGA.Application.Interfaces.Usuarios;
using SIRGA.Application.Services;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Interfaces;
using SIRGA.Identity.Services;
using SIRGA.Identity.Shared.Interfaces;
using SIRGA.Infraestructure.Services;
using SIRGA.Infraestructure.Services.Email;
using SIRGA.Infraestructure.Settings;
using SIRGA.Persistence.Repositories.Usuarios;

namespace SIRGA.IOC
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Repositorios
            services.AddScoped<IEstudianteRepository, EstudianteRepository>();
            services.AddScoped<IProfesorRepository, ProfesorRepository>();

            // servicios de Application
            services.AddScoped<IEstudianteService, EstudianteService>();
            services.AddScoped<IProfesorService, ProfesorService>();

            // servicios de identity
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IIdentityUrlGenerator, IdentityUrlGenerator>();


            // servicios de Infrastructure (Helpers)
            services.AddScoped<IMatriculaGeneratorService, MatriculaGeneratorService>();
            services.AddScoped<IEmailGeneratorService, EmailGeneratorService>();
            services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();
            services.AddScoped<IEmailTemplateGenerator, EmailTemplateGenerator>();

            //email service (depende del ambiente)
            ConfigureEmailService(services, configuration);

            return services;
        }

        private static void ConfigureEmailService(
            IServiceCollection services,
            IConfiguration configuration)
        {
            // Configurar EmailSettings desde appsettings.json
            services.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings")
            );

            var emailMode = configuration.GetValue<string>("EmailSettings:Mode") ?? "Development";

            switch (emailMode.ToLower())
            {
                case "gmail":
                case "mailtrap":
                case "production":
                    // Usar servicio real con MailKit
                    services.AddScoped<IEmailService, EmailService>();
                    break;

                case "development":
                default:
                    // Usar servicio de desarrollo (solo logs)
                    services.AddScoped<IEmailService, DevelopmentEmailService>();
                    break;
            }
        }
    }
}
