using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIRGA.Domain.Entities;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.DbContext;

namespace SIRGA.Persistence.Seeds
{
    public static class TestDataSeeder
    {
        public static async Task SeedTestData(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string lastMatricula = await context.Estudiantes
        .Select(e => e.Matricula)
        .OrderByDescending(m => m)
        .FirstOrDefaultAsync();

            MatriculaGeneratorPrueba.Initialize(lastMatricula??"");

            int currentYear = DateTime.Now.Year;

            var faker = new Bogus.Faker("es");

            // ⭐ Lista fija de especialidades
            /*var especialidades = new List<string>
            {
                "Matemática",
                "Lengua Española",
                "Ciencias Sociales",
                "Ciencias Naturales",
                "Educación Física",
                "Educación Artística",
                "Inglés",
                "Orientación",
                "Informática"
            };

            // 2️⃣ Crear 20 profesores falsos
            for (int i = 0; i < 8; i++)
            {
                var first = faker.Name.FirstName();
                var last = faker.Name.LastName();
                var email = EmailGenerator.GenerateProfesorEmail(first, last);

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    FirstName = first,
                    LastName = last,

                    Gender = faker.PickRandom(new[] { 'M', 'F' }),
                    DateOfBirth = DateOnly.FromDateTime(faker.Date.Between(new DateTime(1968, 1, 1), new DateTime(2000, 12, 31))),
                    Province = faker.Address.State(),
                    Sector = faker.Address.City(),
                    Address = faker.Address.StreetAddress(),
                    PhoneNumber = faker.Phone.PhoneNumber("###-###-####"),

                    IsActive = true,
                    DateOfEntry = DateOnly.FromDateTime(DateTime.Now),
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    MustCompleteProfile = false,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false
                };

                await userManager.CreateAsync(user, "Password123!");

                context.Profesores.Add(new Profesor
                {
                    ApplicationUserId = user.Id,

                    // ⭐ Elegir una especialidad de la lista fija
                    Specialty = faker.PickRandom(especialidades)
                });
            }*/

            // 3️⃣ Crear 32 estudiantes falsos
            for (int i = 0; i < 32; i++)
            {
                var matricula = MatriculaGeneratorPrueba.Generar();
                var yearOfEntry = currentYear.ToString();
                var first = faker.Name.FirstName();
                var last = faker.Name.LastName();
                var email = EmailGenerator.GenerateEstudianteEmail(matricula);

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    FirstName = first,
                    LastName = last,

                    Gender = faker.PickRandom(new[] { 'M', 'F' }),
                    DateOfBirth = DateOnly.FromDateTime(faker.Date.Between(new DateTime(2008, 1, 1), new DateTime(2018, 12, 31))),
                    Province = faker.Address.State(),
                    Sector = faker.Address.City(),
                    Address = faker.Address.StreetAddress(),
                    PhoneNumber = faker.Phone.PhoneNumber("###-###-####"),

                    IsActive = true,
                    DateOfEntry = DateOnly.FromDateTime(DateTime.Now),
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    MustCompleteProfile = false,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false
                };

                await userManager.CreateAsync(user, "Password123!");

                context.Estudiantes.Add(new Estudiante
                {
                    ApplicationUserId = user.Id,
                    Matricula = matricula,

                    MedicalConditions = faker.PickRandom(new[]
    {
        "",
        "Asma",
        "Alergia al maní",
        "Miopía",
        "Ninguna"
    }),

                    MedicalNote = faker.Lorem.Sentence(),

                    EmergencyContactName = faker.Name.FullName(),
                    EmergencyContactPhone = faker.Phone.PhoneNumber("###-###-####")
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
