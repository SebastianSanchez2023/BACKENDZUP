using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZuperBackend.Application.Services.Auth;
using ZuperBackend.Infrastructure.Persistence;
using ZuperBackend.Infrastructure.Services.Auth;

namespace ZuperBackend.Infrastructure.Configuration;

/// <summary>
/// Extensión para registrar servicios de infraestructura en el contenedor de inyección de dependencias.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Agrega servicios de infraestructura (DbContext, Repositories, etc.).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddAuthenticationServices(services);

        return services;
    }

    /// <summary>
    /// Configura la persistencia (DbContext y base de datos).
    /// </summary>
    private static void AddPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ZuperBackendDbContext>(options =>
        {
            // Usar SQL Server Express por defecto
            // Se puede cambiar a PostgreSQL según configuración
            var dbProvider = configuration.GetValue<string>("Database:Provider") ?? "SqlServer";

            if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.MigrationsAssembly("ZuperBackend.Infrastructure"));
            }

            // Habilitar lazy loading (opcional - requiere propiedades virtuales)
            // options.UseLazyLoadingProxies();

            // Habilitar registro detallado en desarrollo
#if DEBUG
            options.LogTo(Console.WriteLine);
#endif
        });
    }

    /// <summary>
    /// Registra servicios de autenticación y autorización
    /// </summary>
    private static void AddAuthenticationServices(IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
