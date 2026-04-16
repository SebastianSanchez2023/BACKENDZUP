using Microsoft.EntityFrameworkCore;
using ZuperBackend.Infrastructure.Configuration;
using ZuperBackend.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de infraestructura (DbContext, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ZuperBackendDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Rutas de prueba
app.MapGet("/api/health", () => Results.Ok(new { status = "API is healthy", timestamp = DateTime.UtcNow }))
    .WithName("Health")
    .WithOpenApi();

app.Run();
