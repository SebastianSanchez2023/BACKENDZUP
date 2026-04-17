using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZuperBackend.Application.Services;
using ZuperBackend.Application.Services.Auth;
using ZuperBackend.Infrastructure.Persistence;
using ZuperBackend.Infrastructure.Services;
using ZuperBackend.Infrastructure.Services.Auth;

namespace ZuperBackend.Infrastructure.Configuration;

/// <summary>
/// Extensión para registrar servicios de infraestructura en el contenedor de inyección de dependencias.
/// 
/// RESPONSABILIDADES:
/// 1. Registrar DbContext (acceso a base de datos)
/// 2. Registrar servicios de autenticación (JWT, login)
/// 3. Registrar servicios de negocio (Asset, Incident, Diagnosis)
/// 4. Configurar AutoMapper para mapeos DTO ↔ Entity
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Agrega servicios de infraestructura (DbContext, Repositories, Services, AutoMapper, etc.).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registra DbContext y configuración de BD
        AddPersistence(services, configuration);
        
        // Registra servicios de autenticación (JWT, Login)
        AddAuthenticationServices(services);
        
        // Registra servicios de negocio (Asset, Incident, Diagnosis)
        AddBusinessServices(services);
        
        // Configura AutoMapper para mapeos Entity ↔ DTO
        AddAutoMapper(services);

        return services;
    }

    /// <summary>
    /// Configura la persistencia (DbContext y base de datos).
    /// 
    /// CICLO DE VIDA: Scoped
    /// RAZÓN: Cada HTTP request obtiene su propio DbContext.
    /// Esto permite cambios transaccionales completos por request.
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
    /// Registra servicios de autenticación y autorización.
    /// 
    /// CICLO DE VIDA: Scoped
    /// </summary>
    private static void AddAuthenticationServices(IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    /// <summary>
    /// Registra servicios de lógica de negocio.
    /// 
    /// CICLO DE VIDA: Transient
    /// RAZÓN: Los servicios no mantienen estado. Reciben DbContext inyectado (Scoped)
    /// 
    /// SERVICIOS REGISTRADOS:
    /// - IAssetService → AssetService: Operaciones CRUD sobre Activos
    /// - IIncidentService → IncidentService: Operaciones CRUD sobre Incidentes
    /// - IDiagnosisService → DiagnosisService: Navegación del árbol de diagnóstico
    /// </summary>
    private static void AddBusinessServices(IServiceCollection services)
    {
        services.AddTransient<IAssetService, AssetService>();
        services.AddTransient<IIncidentService, IncidentService>();
        services.AddTransient<IDiagnosisService, DiagnosisService>();
    }

    /// <summary>
    /// Configura AutoMapper para mapeos Entity → DTO y DTO → Entity.
    /// 
    /// CONCEPTO: AutoMapper es una librería que automatiza la conversión entre tipos.
    /// En lugar de hacer manualmente:
    ///   ❌ MALO:
    ///     var assetDto = new AssetDto 
    ///     { 
    ///         Id = asset.Id, 
    ///         Name = asset.Name, 
    ///         ... 100 líneas más
    ///     };
    /// 
    ///   ✅ BUENO:
    ///     var assetDto = _mapper.Map<AssetDto>(asset);
    /// 
    /// FLUJO:
    /// 1. CreateAutoMapper() busca todas las clases Profile en el proyecto
    /// 2. Cada Profile define reglas de mapeo (CreateAssetDto → Asset, etc)
    /// 3. Se valida en startup que todos los mapeos sean válidos
    /// </summary>
    private static void AddAutoMapper(IServiceCollection services)
    {
        // Busca automáticamente todas las clases Profile en el proyecto
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}
