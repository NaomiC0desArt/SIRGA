using Microsoft.OpenApi.Models;
using SIRGA.Identity.Register;
using SIRGA.IOC;
using SIRGA.Persistence.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SIRGA API",
        Version = "v1",
        Description = "Sistema de Registro y Gestión Académica"
    });
    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de un espacio y el token JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});

builder.Services.AddIdentityLayer(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7095", "http://localhost:5082") // Puerto de tu frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Importante para cookies de autenticación
    });
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.RunIdentitySeeds();
}

if (app.Environment.IsDevelopment() &&
    builder.Configuration.GetValue<bool>("SeedTestData"))
{
    using var scope = app.Services.CreateScope();
    await TestDataSeeder.SeedTestData(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIRGA API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
